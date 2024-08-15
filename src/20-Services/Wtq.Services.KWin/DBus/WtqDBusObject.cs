using Tmds.DBus;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Utils;

namespace Wtq.Services.KWin.DBus;

internal class WtqDBusObject(
	IWtqBus bus)
	: IWtqDBusObject
{
	public static readonly ObjectPath Path = new("/wtq/kwin");

	private readonly ConcurrentDictionary<Guid, KWinResponseWaiter> _waiters = new();
	private readonly ILogger _log = Log.For<WtqDBusObject>();
	private readonly IWtqBus _bus = Guard.Against.Null(bus);

	public ObjectPath ObjectPath => Path;

	public Task SendResponseAsync(string responderIdStr, string payloadJson)
	{
		responderIdStr ??= string.Empty;
		payloadJson ??= string.Empty;

		_log.LogInformation(
			"{MethodName}({ResponderId}, {PayloadJson})",
			nameof(SendResponseAsync),
			responderIdStr,
			payloadJson[0..25]);

		if (!Guid.TryParse(responderIdStr, out var responderId))
		{
			_log.LogWarning("Could not parse responder id {ResponderId} as a guid", responderIdStr);
			return Task.CompletedTask;
		}

		if (!_waiters.TryRemove(responderId, out var waiter))
		{
			_log.LogWarning("Could not find response waiter with id {ResponderId}", responderId);
			return Task.CompletedTask;
		}

		waiter.SetResult(payloadJson);

		return Task.CompletedTask;
	}

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

	public KWinResponseWaiter CreateResponseWaiter(Guid id)
	{
		var waiter = new KWinResponseWaiter(id, () => _waiters.TryRemove(id, out _));

		_waiters.TryAdd(id, waiter);

		return waiter;
	}
}