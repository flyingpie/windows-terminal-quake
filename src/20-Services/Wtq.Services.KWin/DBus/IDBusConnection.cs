using Tmds.DBus;
using Connection = Tmds.DBus.Protocol.Connection;

namespace Wtq.Services.KWin.DBus;

public interface IDBusConnection
{
	public Connection ClientConnection { get; }

	public Tmds.DBus.Connection ServerConnection { get; }

	Task RegisterServiceAsync(string serviceName, IDBusObject serviceObject);
}