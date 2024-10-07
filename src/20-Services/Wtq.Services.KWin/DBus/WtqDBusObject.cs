using System.Text.Json;
using Tmds.DBus;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Services.KWin.Dto;

namespace Wtq.Services.KWin.DBus;

internal sealed class WtqDBusObject
	: IWtqDBusObject // TODO: Add second interface for internal-facing stuff?
{
	public static readonly ObjectPath Path = new("/wtq/kwin");

	private readonly ConcurrentQueue<CommandInfo> _commandQueue = new();
	private readonly ConcurrentDictionary<Guid, KWinResponseWaiter> _waiters = new();
	private readonly ILogger _log = Log.For<WtqDBusObject>();

	private readonly IWtqBus _bus;
	private readonly IDBusConnection _dbus;
	private readonly Initializer _init;

	public WtqDBusObject(
		IDBusConnection dbus,
		IWtqBus bus)
	{
		_bus = Guard.Against.Null(bus);
		_dbus = Guard.Against.Null(dbus);
		_init = new(InitializeAsync);
	}

	public ObjectPath ObjectPath => Path;

	public void Dispose()
	{
		_init.Dispose();
	}

	public async Task InitAsync()
	{
		await _init.InitializeAsync();
	}

	private async Task InitializeAsync()
	{
		await _dbus.RegisterServiceAsync("wtq.svc", this).ConfigureAwait(false);
	}

	public async Task<ResponseInfo> SendCommandAsync(CommandInfo cmdInfo)
	{
		_log.LogInformation("{MethodName} command: {Command}", nameof(SendCommandAsync), cmdInfo);

		await _init.InitializeAsync().NoCtx();

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

		// Wait for response
		// TODO: Cancellation token.
		return await waiter.Task.NoCtx();
	}

	/// <inheritdoc/>
	public async Task<string> GetNextCommandAsync(string a, string b, string c)
	{
		_log.LogInformation($"DoTheThing('{a}', '{b}', '{c}')");

		await _init.InitializeAsync().NoCtx();

		if (_commandQueue.TryDequeue(out var cmdInfo))
		{
			_log.LogInformation("Send command '{Command}' to KWin", cmdInfo);
			return JsonSerializer.Serialize(cmdInfo);
		}

		_log.LogInformation("No command in queue");
		await Task.Delay(TimeSpan.FromMilliseconds(25));

		return JsonSerializer.Serialize(new CommandInfo()
		{
			Type = "NOOP",
			// msg = "Dooods!",
			Params = new
			{
				x = 42,
			},
		});
	}

	/// <inheritdoc/>
	public async Task SendResponseAsync(string respInfoStr)
	{
		await _init.InitializeAsync().NoCtx();

		var respInfo = JsonSerializer.Deserialize<ResponseInfo>(respInfoStr);

		var hasResponder = _waiters.TryGetValue(respInfo.ResponderId, out var responder);

		if (!hasResponder)
		{
			_log.LogWarning("Could not find response waiter with id {ResponderId}", respInfo.ResponderId);
			return;
		}

		if (!_waiters.TryRemove(respInfo.ResponderId, out var waiter))
		{
			_log.LogWarning("Could not find response waiter with id {ResponderId}", respInfo.ResponderId);
			return;
		}

		waiter.SetResult(respInfo);
	}

	/// <inheritdoc/>
	public async Task OnPressShortcutAsync(string modStr, string keyStr)
	{
		await _init.InitializeAsync().NoCtx();

		_log.LogInformation(
			"{MethodName}({Modifier}, {Key})",
			nameof(OnPressShortcutAsync),
			modStr,
			keyStr);

		Enum.TryParse<Keys>(keyStr, ignoreCase: true, out var key);
		Enum.TryParse<KeyModifiers>(modStr, ignoreCase: true, out var mod);

		_bus.Publish(
			new WtqHotKeyPressedEvent()
			{
				Key = key,
				Modifiers = mod,
			});
	}
}