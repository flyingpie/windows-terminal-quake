using Microsoft.VisualStudio.Threading;
using System.Text.Json;
using Tmds.DBus;
using Wtq.Events;
using Wtq.Services.KWin.Dto;
using Wtq.Services.KWin.Exceptions;
using Wtq.Services.KWin.Scripting;

namespace Wtq.Services.KWin.DBus;

internal sealed class WtqDBusObject : WtqHostedService, IWtqDBusObject
{
	private readonly IKWinScriptService _scriptService;
	private const string ServiceName = "nl.flyingpie.wtq.svc";

	private static readonly ObjectPath _path = new("/wtq/kwin");

	/// <summary>
	/// Packaged KWin script, that's inside the WTQ binaries folder.
	/// </summary>
	private static readonly string _pathToWtqKwinJsSrc = WtqPaths.GetPathRelativeToWtqAppDir("wtq.kwin.js");

	/// <summary>
	/// Path to KWin script in the XDG cache folder, that is reachable by both sandboxed WTQ, and KWin.<br/>
	/// This is necessary when running WTQ as a Flatpak, where KWin can't see the files, since they're sandboxed.
	/// </summary>
	private static readonly string _pathToWtqKwinJsCache = Path.Combine(WtqPaths.GetWtqTempDir(), "wtq.kwin.js");

	private readonly CancellationTokenSource _cts = new();
	private readonly AsyncAutoResetEvent _res = new(false);
	private readonly ConcurrentQueue<CommandInfo> _commandQueue = new();
	private readonly ConcurrentDictionary<Guid, KWinResponseWaiter> _waiters = new();
	private readonly ILogger _log = Log.For<WtqDBusObject>();

	private readonly IWtqBus _bus;
	private readonly IDBusConnection _dbus;

	private readonly List<Func<KeySequence, Task>> _onPressShortcutHandlers = [];

	private KWinScript? _script;

	private readonly RecurringTask _noopLoop;

	public WtqDBusObject(
		IDBusConnection dbus,
		IKWinScriptService scriptService,
		IWtqBus bus)
	{
		_bus = Guard.Against.Null(bus);
		_dbus = Guard.Against.Null(dbus);
		_scriptService = Guard.Against.Null(scriptService);

		// The DBus calls from wtq.kwin need to get occasional commands, otherwise the request times out,
		// and the connection is dropped.
		_noopLoop = new(
			$"{nameof(WtqDBusObject)}.NoOpLoop",
			TimeSpan.FromSeconds(10), // Timeout is after a minute or so.
			async ct => await SendCommandAsync("NOOP", null, ct).NoCtx());
	}

	public ObjectPath ObjectPath => _path;

	protected override async Task OnInitAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Setting up WTQ DBus service");

		// Register this object as a DBus service.
		await _dbus.RegisterServiceAsync(ServiceName, this).NoCtx();

		// Load KWin script.
		// Note that we're copying the KWin script to the cache dir, as that will be available to both the host and the sandbox, in the case we're running as a Flatpak.
		// For non-Flatpak, we could use the path to the KWin script directly, but that cannot be seen by KWin since KWin can't see in our sandbox.
		_log.LogDebug("Copying KWin script from '{Src}' to '{Dst}'", _pathToWtqKwinJsSrc, _pathToWtqKwinJsCache);

		File.Copy(_pathToWtqKwinJsSrc, _pathToWtqKwinJsCache, overwrite: true);

		_script = await _scriptService.LoadScriptAsync(_pathToWtqKwinJsCache).NoCtx();
	}

	protected override async Task OnStartAsync(CancellationToken cancellationToken)
	{
		// Start NOOP loop.
		_noopLoop.Start();
	}

	protected override async ValueTask OnDisposeAsync()
	{
		await _script!.DisposeAsync();
		await _cts.CancelAsync();
	}

	public Task LogAsync(string level, string msg)
	{
		_log.LogTrace("[wtq.kwin.js] {Level} {Message}", level, msg);

		return Task.CompletedTask;
	}

	public Task<ResponseInfo> SendCommandAsync(
		string commandType,
		object? parameters,
		CancellationToken cancellationToken)
		=> SendCommandAsync(
			new CommandInfo(commandType)
			{
				Params = parameters,
			},
			cancellationToken);

	public async Task<ResponseInfo> SendCommandAsync(
		CommandInfo cmdInfo,
		CancellationToken cancellationToken)
	{
		_log.LogTrace("{MethodName} command: {Command}", nameof(SendCommandAsync), cmdInfo);

		// Add response waiter.
		var id = cmdInfo.ResponderId;
		using var waiter = new KWinResponseWaiter(id, () => _waiters.TryRemove(id, out _));
		if (!_waiters.TryAdd(id, waiter))
		{
			_log.LogError("Command with id '{Id}' already queued", id);
			throw new InvalidOperationException($"Command with id '{id}' already queued.");
		}

		// Queue command
		_commandQueue.Enqueue(cmdInfo);
		_res.Set();

		// Wait for response
		try
		{
			return await waiter.Task
				.WithCancellation(cancellationToken)
				.WithTimeout(Debugger.IsAttached ? TimeSpan.FromMinutes(60) : TimeSpan.FromSeconds(1))
				.NoCtx();
		}
		catch (TaskCanceledException)
		{
			throw new KWinException($"Task canceled while attempting to send KWin command '{cmdInfo}'");
		}
		catch (TimeoutException ex)
		{
			throw new KWinException($"Timeout while attempting to send KWin command '{cmdInfo}': {ex.Message}", ex);
		}
	}

	/// <inheritdoc/>
	public async Task<string> GetNextCommandAsync()
	{
		while (!_cts.IsCancellationRequested)
		{
			// See if we have a command on the queue to send back.
			if (_commandQueue.TryDequeue(out var cmdInfo))
			{
				_log.LogTrace("Send command '{Command}' to KWin", cmdInfo);
				return JsonSerializer.Serialize(cmdInfo);
			}

			// If not, wait for one to be queued.
			await _res.WaitAsync(_cts.Token).NoCtx();
		}

		_log.LogTrace("Sending 'STOPPING' command to KWin script");
		return JsonSerializer.Serialize(CommandInfo.Stopping);
	}

	/// <inheritdoc/>
	public Task SendResponseAsync(string respInfoStr)
	{
		var respInfo = JsonSerializer.Deserialize<ResponseInfo>(respInfoStr);

		_log.LogTrace("Got response {Response}", respInfo);

		var hasResponder = _waiters.TryGetValue(respInfo.ResponderId, out var responder);

		if (!hasResponder)
		{
			_log.LogWarning("Could not find response waiter with id {ResponderId}", respInfo.ResponderId);
			return Task.CompletedTask;
		}

		if (!_waiters.TryRemove(respInfo.ResponderId, out var waiter))
		{
			_log.LogWarning("Could not find response waiter with id {ResponderId}", respInfo.ResponderId);
			return Task.CompletedTask;
		}

		if (respInfo.Exception != null)
		{
			waiter.SetException(respInfo.Exception);
		}
		else
		{
			waiter.SetResult(respInfo);
		}

		return Task.CompletedTask;
	}

	public void OnPressShortcut(Func<KeySequence, Task> handler)
	{
		Guard.Against.Null(handler);

		_onPressShortcutHandlers.Add(handler);
	}

	/// <inheritdoc/>
	public async Task OnPressShortcutAsync(
		string name,
		string modStr,
		string keyCharStr,
		string keyCodeStr)
	{
		_log.LogInformation(
			"{MethodName}({Name}, {Modifier}, {KeyChar}, {KeyCode})",
			nameof(OnPressShortcutAsync),
			name,
			modStr,
			keyCharStr,
			keyCodeStr);

		Enum.TryParse<KeyModifiers>(modStr, ignoreCase: true, out var mod);
		Enum.TryParse<KeyCode>(keyCodeStr, ignoreCase: true, out var key);

		var keySeq = new KeySequence(mod, keyCharStr, key);

		await Task.WhenAll(_onPressShortcutHandlers.Select(h => h(keySeq)));
	}

	public Task ToggleAppAsync(string appName)
	{
		_bus.Publish(new WtqAppToggledEvent(appName));

		return Task.CompletedTask;
	}
}