using Tmds.DBus;

namespace Wtq.Services.KWin.DBus;

/// <summary>
/// Contains methods used to talk to, from KWin scripts.
/// </summary>
[DBusInterface("wtq.kwin")]
public interface IWtqDBusObject : IDBusObject
{
	/// <summary>
	/// Called when a shortcut has been pressed.
	/// </summary>
	Task OnPressShortcutAsync(string modStr, string keyStr);

	/// <summary>
	/// Generic callback handler, responses are formatted as JSON.
	/// </summary>
	/// TODO: Can we drop the request/response thing and move to pure events?
	Task SendResponseAsync(string responderIdStr, string payloadJson);
}