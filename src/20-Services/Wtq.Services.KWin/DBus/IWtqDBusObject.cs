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

	/// <summary>
	/// Called when a shortcut has been pressed.<br/>
	/// TODO: Would like to remove this, and do shortcuts through DBus (without KWin script). Didn't get that working just yet.
	/// </summary>
	Task OnPressShortcutAsync(string modStr, string keyCharStr, string keyCodeStr);

	Task ToggleAppAsync(string appName);
}