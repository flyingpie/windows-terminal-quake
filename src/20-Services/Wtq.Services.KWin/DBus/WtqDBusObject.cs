using System.Collections.Concurrent;
using Tmds.DBus;
using Wtq.Utils;

namespace Wtq.Services.KWin.DBus;

internal class WtqDBusObject : IWtqDBusObject
{
	public static readonly ObjectPath Path = new("/wtq/kwin");

	private readonly ConcurrentDictionary<Guid, KWinResponseWaiter> _waiters = new();
	private readonly ILogger _log = Log.For<WtqDBusObject>();

	public ObjectPath ObjectPath => Path;

	public Task SendResponseAsync(string responderIdStr, string payloadJson)
	{
		if (!Guid.TryParse(responderIdStr, out var responderId))
		{
			_log.LogWarning("Could not parse responder id '{ResponderId}' as a guid", responderIdStr);
			return Task.CompletedTask;
		}

		if (!_waiters.TryRemove(responderId, out var waiter))
		{
			_log.LogWarning("Could not find response waiter with id '{ResponderId}'", responderId);
			return Task.CompletedTask;
		}

		waiter.SetResult(payloadJson);

		return Task.CompletedTask;
	}

	public KWinResponseWaiter CreateResponseWaiter(Guid id)
	{
		var waiter = new KWinResponseWaiter(id, () => _waiters.TryRemove(id, out _));

		_waiters.TryAdd(id, waiter);

		return waiter;
	}
}