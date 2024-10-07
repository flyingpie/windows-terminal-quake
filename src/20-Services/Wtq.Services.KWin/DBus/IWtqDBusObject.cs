using Tmds.DBus;

namespace Wtq.Services.KWin.DBus;

/// <summary>
/// Contains methods used to talk to, from KWin scripts.
/// </summary>
[DBusInterface("wtq.kwin")]
public interface IWtqDBusObject : IDBusObject, IDisposable
{
	Task InitAsync();

	Task LogAsync(string level, string msg);

	/// <summary>
	/// Responses to commands (as passed through <seealso cref="GetNextCommandAsync"/>), are dropped here.
	/// </summary>
	/// TODO: Can we drop the request/response thing and move to pure events?
	Task SendResponseAsync(string respInfoStr);

	/// <summary>
	/// Ask WTQ for the next command to execute in the KWin script.
	/// </summary>
	Task<string> GetNextCommandAsync(string a, string b, string c);

	/// <summary>
	/// Called when a shortcut has been pressed.<br/>
	/// TODO: Would like to remove this, and do shortcuts through DBus (without KWin script). Didn't get that working just yet.
	/// </summary>
	Task OnPressShortcutAsync(string modStr, string keyStr);
}