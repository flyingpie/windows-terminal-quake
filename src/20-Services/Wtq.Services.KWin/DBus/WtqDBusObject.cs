using Microsoft.VisualStudio.Threading;
using System.Text.Json;
using Tmds.DBus;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Services.KWin.Dto;
using Wtq.Services.KWin.Exceptions;

namespace Wtq.Services.KWin.DBus;

internal sealed class WtqDBusObject(
	IDBusConnection dbus,
	IWtqBus bus)
	: System.IAsyncDisposable, IWtqDBusObject
{
	private const string ServiceName = "wtq.svc";

	private static readonly ObjectPath _path = new("/wtq/kwin");

	private readonly CancellationTokenSource _cts = new();
	private readonly AsyncAutoResetEvent _res = new(false);
	private readonly ConcurrentQueue<CommandInfo> _commandQueue = new();
	private readonly ConcurrentDictionary<Guid, KWinResponseWaiter> _waiters = new();
	private readonly ILogger _log = Log.For<WtqDBusObject>();

	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private readonly IDBusConnection _dbus = Guard.Against.Null(dbus);

	private readonly InitLock _lock = new();
	private readonly List<Func<KeySequence, Task>> _onPressShortcutHandlers = [];

	private Worker? _loop;

	public ObjectPath ObjectPath => _path;

	public async Task InitAsync()
	{
		await _lock
			.InitAsync(async () =>
			{
				_log.LogInformation("Setting up WTQ DBus service");

				// Register this object as a DBus service.
				await _dbus.RegisterServiceAsync(ServiceName, this).NoCtx();

				// Start NOOP loop.
				StartNoOpLoop();
			})
			.NoCtx();
	}

	public async ValueTask DisposeAsync()
	{
		_cts.Dispose();
		_lock.Dispose();

		await (_loop?.DisposeAsync() ?? ValueTask.CompletedTask).NoCtx();
	}

	public Task LogAsync(string level, string msg)
	{
		// TODO
		_log.LogDebug("{Level} {Message}", level, msg);

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
		await InitAsync().NoCtx();

		_log.LogDebug("{MethodName} command: {Command}", nameof(SendCommandAsync), cmdInfo);

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
				.WithTimeout(TimeSpan.FromSeconds(1))
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
	public async Task OnPressShortcutAsync(string modStr, string keyCharStr, string keyCodeStr)
	{
		_log.LogInformation(
			"{MethodName}({Modifier}, {KeyChar}, {KeyCode})",
			nameof(OnPressShortcutAsync),
			modStr,
			keyCharStr,
			keyCodeStr);

		Enum.TryParse<KeyModifiers>(modStr, ignoreCase: true, out var mod);
		Enum.TryParse<Keys>(keyCodeStr, ignoreCase: true, out var key);

		var keySeq = new KeySequence()
		{
			Modifiers = mod,
			KeyChar = keyCharStr,
			KeyCode = key,
		};

		await Task.WhenAll(_onPressShortcutHandlers.Select(h => h(keySeq)));
	}

	public Task ToggleAppAsync(string appName)
	{
		_bus.Publish(new WtqAppToggledEvent(appName));

		return Task.CompletedTask;
	}

	/// <summary>
	/// The DBus calls from wtq.kwin need to get occasional commands, otherwise the request times out,
	/// and the connection is dropped.
	/// </summary>
	private void StartNoOpLoop()
	{
		_loop = new(
			$"{nameof(WtqDBusObject)}.{nameof(StartNoOpLoop)}",
			async ct => await SendCommandAsync("NOOP", null, ct).NoCtx(),
			TimeSpan.FromSeconds(10));
	}
}