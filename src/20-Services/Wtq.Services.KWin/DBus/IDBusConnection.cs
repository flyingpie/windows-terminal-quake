using Tmds.DBus;

namespace Wtq.Services.KWin.DBus;

/// <summary>
/// Wraps both a client- and a server connection to DBus.<br/>
/// The client connection is used for sending requests to DBus, the server one is used to register DBus object.
/// </summary>
internal interface IDBusConnection : IDisposable
{
	Task<KWin> GetKWinAsync();

	Task<KWinService> GetKWinServiceAsync();

	Task<Scripting> GetScriptingAsync();

	/// <summary>
	/// Register an object that exposes methods that can be called by other processes.
	/// </summary>
	Task RegisterServiceAsync(string serviceName, IDBusObject serviceObject);
}