using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Tmds.DBus;
using Wtq.Configuration;
using Wtq.Events;

namespace Wtq.Services.KWin.DBus;

public class CommandInfo
{
	public CommandInfo()
	{

	}

	public CommandInfo(string type)
	{
		Type = type;
	}

	/// <summary>
	/// The command we want to execute, like "get window list" and "set window opacity".
	/// </summary>
	[JsonPropertyName("type")]
	public string Type { get; set; }

	/// <summary>
	/// Any parameters that accompany the command, like where to move a window to,
	/// or what opacity to set a window to.
	/// </summary>
	[JsonPropertyName("params")]
	public object Params { get; set; }

	/// <summary>
	/// Used to correlate any responses coming back from the KWin script.<br/>
	/// Note that not all commands result in a response.
	/// </summary>
	[JsonPropertyName("responderId")]
	public Guid ResponderId { get; set; } = Guid.NewGuid();

	public override string ToString() => $"[{Type}]";
}

public class ResponseInfo
{
	[JsonPropertyName("responderId")]
	public Guid ResponderId { get; set; }

	[JsonPropertyName("params")]
	public JsonElement Params { get; set; }

	public T GetParamsAs<T>()
	{
		return Params.Deserialize<T>();
	}
}

internal class WtqDBusObject(
	IWtqBus bus)
	: IWtqDBusObject // TODO: Add second interface for internal-facing stuff?
{
	public static readonly ObjectPath Path = new("/wtq/kwin");

	private readonly ConcurrentQueue<CommandInfo> _commandQueue = new();
	private readonly ConcurrentDictionary<Guid, KWinResponseWaiter> _waiters = new();
	private readonly ILogger _log = Log.For<WtqDBusObject>();
	private readonly IWtqBus _bus = Guard.Against.Null(bus);

	public ObjectPath ObjectPath => Path;

	public async Task<ResponseInfo> SendCommandAsync(CommandInfo cmdInfo)
	{
		_log.LogInformation("{MethodName} command: {Command}", nameof(SendCommandAsync), cmdInfo);
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

		if (_commandQueue.TryDequeue(out var cmdInfo))
		{
			_log.LogInformation("Send command '{Command}' to KWin", cmdInfo);
			return JsonSerializer.Serialize(cmdInfo);
		}

		_log.LogInformation("No command in queue");
		await Task.Delay(TimeSpan.FromMilliseconds(500));

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
	public Task SendResponseAsync(string respInfoStr)
	{
		var respInfo = JsonSerializer.Deserialize<ResponseInfo>(respInfoStr);

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

		waiter.SetResult(respInfo);

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
			new WtqHotKeyPressedEvent()
			{
				Key = key,
				Modifiers = mod,
			});

		return Task.CompletedTask;
	}
}