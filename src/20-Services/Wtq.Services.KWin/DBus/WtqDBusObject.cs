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
	: IAsyncInitializable, IWtqDBusObject
{
	private static readonly ObjectPath _path = new("/wtq/kwin");

	private readonly CancellationTokenSource _cts = new();
	private readonly AsyncAutoResetEvent _res = new(false);
	private readonly ConcurrentQueue<CommandInfo> _commandQueue = new();
	private readonly ConcurrentDictionary<Guid, KWinResponseWaiter> _waiters = new();
	private readonly ILogger _log = Log.For<WtqDBusObject>();

	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private readonly IDBusConnection _dbus = Guard.Against.Null(dbus);

	public int InitializePriority => 10;

	public ObjectPath ObjectPath => _path;

	public async Task InitializeAsync()
	{
		await _dbus.RegisterServiceAsync("wtq.svc", this).NoCtx();

		StartNoOpLoop();
	}

	public void Dispose()
	{
		_cts.Dispose();
		_dbus.Dispose();
	}

	public Task LogAsync(string level, string msg)
	{
		// TODO
		_log.LogDebug("{Level} {Message}", level, msg);

		return Task.CompletedTask;
	}

	public Task<ResponseInfo> SendCommandAsync(string commandType, object? parameters = null)
		=> SendCommandAsync(new CommandInfo(commandType) { Params = parameters ?? new() });

	public async Task<ResponseInfo> SendCommandAsync(CommandInfo cmdInfo)
	{
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
			return await waiter.Task.TimeoutAfterAsync(TimeSpan.FromSeconds(1)).NoCtx();
		}
		catch (TimeoutException ex)
		{
			throw new KWinException($"Timeout while attempting to send KWin command '{cmdInfo}': {ex.Message}", ex);
		}
	}

	/// <inheritdoc/>
	public async Task<string> GetNextCommandAsync()
	{
		while (true)
		{
			// See if we have a command on the queue to send back.
			if (_commandQueue.TryDequeue(out var cmdInfo))
			{
				_log.LogTrace("Send command '{Command}' to KWin", cmdInfo);
				return JsonSerializer.Serialize(cmdInfo);
			}

			// If not, wait for one to be queued.
			await _res.WaitAsync().NoCtx();
		}
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

	/// <inheritdoc/>
	public Task OnPressShortcutAsync(string modStr, string keyStr)
	{
		_log.LogInformation(
			"{MethodName}({Modifier}, {Key})",
			nameof(OnPressShortcutAsync),
			modStr,
			keyStr);

		Enum.TryParse<Keys>(keyStr, ignoreCase: true, out var key);
		Enum.TryParse<KeyModifiers>(modStr, ignoreCase: true, out var mod);

		_bus.Publish(
			new WtqHotkeyPressedEvent()
			{
				Key = key,
				Modifiers = mod,
			});

		_bus.Publish(new WtqUIRequestedEvent());

		return Task.CompletedTask;
	}

	/// <summary>
	/// The DBus calls from wtq.kwin need to get occasional commands, otherwise the request times out,
	/// and the connections is dropped.
	/// </summary>
	private void StartNoOpLoop()
	{
		// TODO: Generalize loop.
		_ = Task.Run(async () =>
		{
			while (!_cts.IsCancellationRequested)
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
}