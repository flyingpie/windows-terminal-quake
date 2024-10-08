using Microsoft.VisualStudio.Threading;
using System.Text.Json;
using Tmds.DBus;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Services.KWin.Dto;
using Wtq.Services.KWin.Exceptions;

namespace Wtq.Services.KWin.DBus;

internal sealed class WtqDBusObject
	: IWtqDBusObject // TODO: Add second interface for internal-facing stuff?
{
	private static readonly ObjectPath _path = new("/wtq/kwin");

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
		_init = new Initializer<WtqDBusObject>(InitializeAsync);
	}

	public ObjectPath ObjectPath => _path;

	public void Dispose()
	{
		_dbus.Dispose();
		_init.Dispose();
	}

	public async Task InitAsync()
	{
		await _init.InitAsync().NoCtx();
	}

	private readonly CancellationTokenSource _cts = new();

	private async Task InitializeAsync()
	{
		await _dbus.RegisterServiceAsync("wtq.svc", this).ConfigureAwait(false);

	}

	/// <summary>
	/// The DBus calls from wtq.kwin need to get occasional commands, otherwise the request times out,
	/// and the connections is dropped.
	/// </summary>
	private void StartNoOpLoop()
	{
		_ = Task.Run(async () =>
		{
			while(!_cts.IsCancellationRequested)
			{
				try
				{
					await SendCommandAsync("NOOP").NoCtx();
				}
				catch (Exception ex)
				{
					_log.LogError(ex, "Error while sending NO_OP to wtq.kwin: {Message}", ex.Message);
				}

				await Task.Delay(TimeSpan.FromSeconds(10)).NoCtx();
			}
		});
	}

	public Task LogAsync(string level, string msg)
	{
		// TODO
		_log.LogInformation($"{level} {msg}");

		return Task.CompletedTask;
	}

	public Task<ResponseInfo> SendCommandAsync(string commandType, object? parameters = null)
		=> SendCommandAsync(new CommandInfo(commandType) { Params = parameters ?? new() });

	public async Task<ResponseInfo> SendCommandAsync(CommandInfo cmdInfo)
	{
		_log.LogInformation("{MethodName} command: {Command}", nameof(SendCommandAsync), cmdInfo);

		await _init.InitAsync().NoCtx();

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
		// TODO: Cancellation token.
		try
		{
			return await waiter.Task.TimeoutAfter(TimeSpan.FromSeconds(1)).NoCtx();
		}
		catch (TimeoutException ex)
		{
			throw new KWinException($"Timeout while attempting to send KWin command '{cmdInfo}': {ex.Message}", ex);
		}
	}

	private AsyncAutoResetEvent _res = new(false);

	/// <inheritdoc/>
	public async Task<string> GetNextCommandAsync(string a, string b, string c)
	{
		// _log.LogInformation($"DoTheThing('{a}', '{b}', '{c}')");

		await _init.InitAsync().NoCtx();

		while (true)
		{
			// See if we have a command on the queue to send back.
			if (_commandQueue.TryDequeue(out var cmdInfo))
			{
				_log.LogInformation("Send command '{Command}' to KWin", cmdInfo);
				return JsonSerializer.Serialize(cmdInfo);
			}

			// If not, wait for one to be queued.
			// TODO: Can this timeout, do we need to drop NOOPs?
			await _res.WaitAsync().NoCtx();
		}

		// _log.LogInformation("No command in queue");
		// await Task.Delay(TimeSpan.FromMilliseconds(100));
		//
		// return JsonSerializer.Serialize(new CommandInfo()
		// {
		// 	Type = "NOOP",
		// 	// msg = "Dooods!",
		// 	Params = new
		// 	{
		// 		x = 42,
		// 	},
		// });
	}

	/// <inheritdoc/>
	public async Task SendResponseAsync(string respInfoStr)
	{
		await _init.InitAsync().NoCtx();

		var respInfo = JsonSerializer.Deserialize<ResponseInfo>(respInfoStr);

		_log.LogInformation("Got response {Response}", respInfo);

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

		if (respInfo.Exception != null)
		{
			waiter.SetException(respInfo.Exception);
		}
		else
		{
			waiter.SetResult(respInfo);
		}
	}

	/// <inheritdoc/>
	public async Task OnPressShortcutAsync(string modStr, string keyStr)
	{
		await _init.InitAsync().NoCtx();

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