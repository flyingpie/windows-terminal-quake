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
	/// (Windows only) Switches the hotkey subsystem to using SharpHook, instead of registering through an invisible WinForm's window message loop.<br/>
	/// Enables more keys (such as the "Windows", or "Meta", or "Super" modifier).<br/>
	/// <br/>
	/// While implementing this, some subtle issues were encountered. Although all the known ones have been fixed, it's enough of a change that we're feature-flagging it for a while.
	/// </summary>
	public bool SharpHook { get; set; }

	/// <summary>
	/// When enabled, allows one WTQ app per screen instead of one globally.<br/>
	/// Toggling an app on a screen only closes other WTQ apps on that same screen, allowing apps on
	/// different monitors to remain open simultaneously.<br/>
	/// <br/>
	/// When disabled, restores the legacy behavior where toggling any app closes the previously-open app
	/// regardless of which screen it was on.
	/// </summary>
	public bool PerScreenApps { get; set; } = true;
}