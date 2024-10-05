using Tmds.DBus;
using Connection = Tmds.DBus.Protocol.Connection;

namespace Wtq.Services.KWin.DBus;

/// <summary>
/// Wraps both a client- and a server connection to DBus.<br/>
/// The client connection is used for sending requests to DBus, the server one is used to register DBus object.
/// </summary>
public interface IDBusConnection
{
	/// <summary>
	/// Client connection, used to send requests to DBus.
	/// </summary>
	Connection ClientConnection { get; }

	/// <summary>
	/// Server connection, used to register DBus objects.
	/// </summary>
	Tmds.DBus.Connection ServerConnection { get; }

	/// <summary>
	/// Register an object that exposes methods that can be called by other processes.
	/// </summary>
	Task RegisterServiceAsync(string serviceName, IDBusObject serviceObject);
}