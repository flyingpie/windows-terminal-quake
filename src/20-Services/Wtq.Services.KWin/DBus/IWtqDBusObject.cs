using Tmds.DBus;

namespace Wtq.Services.KWin.DBus;

/// <summary>
/// Contains methods used to talk to, from KWin scripts.
/// </summary>
[DBusInterface("wtq.kwin")]
public interface IWtqDBusObject : IDBusObject
{
	Task LogAsync(string level, string msg);

	/// <summary>
	/// Responses to commands (as passed through <seealso cref="GetNextCommandAsync"/>), are dropped here.
	/// </summary>
	Task SendResponseAsync(string respInfoStr);

	/// <summary>
	/// Ask WTQ for the next command to execute in the KWin script.
	/// </summary>
	Task<string> GetNextCommandAsync();

	Task ToggleAppAsync(string appName);
}
