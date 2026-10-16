namespace Wtq.Services;

/// <summary>
/// Remembers which window WTQ started for each app, across WTQ restarts.<br/>
/// <br/>
/// Used by <see cref="AttachMode.StartOnly"/>, so that WTQ can re-attach to a window it started earlier
/// (e.g. after WTQ itself was restarted), while still never attaching to windows it did not start.
/// </summary>
public interface IWtqStartedWindowsStore
{
	/// <summary>
	/// Returns the id of the window that was started for the app with the specified <paramref name="appName"/>, or null if none is known.
	/// </summary>
	string? GetWindowId(string appName);

	/// <summary>
	/// Stores the id of the window that was started for the app with the specified <paramref name="appName"/>.
	/// </summary>
	void SetWindowId(string appName, string windowId);

	/// <summary>
	/// Forgets the window that was started for the app with the specified <paramref name="appName"/> (if any).
	/// </summary>
	void RemoveWindowId(string appName);
}
