using Tmds.DBus;

namespace Wtq.Services.KWin.DBus;

[DBusInterface("wtq.kwin")]
public interface IWtqDBusObject : IDBusObject
{
	Task SendResponseAsync(string responderIdStr, string payloadJson);
}