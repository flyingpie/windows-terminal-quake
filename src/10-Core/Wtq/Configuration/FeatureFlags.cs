namespace Wtq.Configuration;

/// <summary>
/// Sometimes functionality is added or changed that carries more risk of introducing bugs.<br/>
/// <br/>
/// For these cases, such functionality can be put behind a "feature flag", which makes them opt-in or opt-out.<br/>
/// That way, we can still merge to master, and make it part of the stable release version (reducing branches and dev builds and what not),
/// but still have a way back should things go awry.
/// </summary>
public class FeatureFlags
{
	/// <summary>
	/// (Windows only) Switches to finding and capturing windows starting from the _window_, instead of from the _process.<br/>
	/// Enables attaching to windows other than an application's main window (useful for browsers, for example), and other
	/// cases where a single process spawns a bunch of windows (like the Windows file explorer, which is part of the "explorer" process).<br/>
	/// <br/>
	/// Pretty radically different from the previous implementation, so we're feature-flagging it for a while.
	/// </summary>
	public bool NewWindowCapture { get; set; }

	/// <summary>
	/// (Windows only) Switches the hotkey subsystem to using SharpHook, instead of registering through an invisible WinForm's window message loop.<br/>
	/// Enables more keys (such as the "Windows", or "Meta", or "Super" key).<br/>
	/// <br/>
	/// While implementing this, some subtle issues were encountered. Although all the known ones have been fixed, it's enough of a change that we're feature-flagging it for a while.
	/// </summary>
	public bool SharpHook { get; set; }
}