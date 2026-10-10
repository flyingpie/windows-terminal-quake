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
	/// (Windows only) Use SharpHook (a low-level keyboard hook) for hotkeys, instead of registering them through an invisible WinForms window (<b>RegisterHotKey</b>).<br/>
	/// <br/>
	/// <b>On</b> (default): supports more keys (such as the "Windows", or "Meta", or "Super" modifier), and hotkeys can be specified as a <b>key character</b>.<br/>
	/// <b>Off</b>: hotkeys also work while an elevated (administrator) window has focus, and don't depend on the keyboard hook. Hotkeys must be specified as a <b>key code</b> (e.g. "Oem3"), not as a key character.<br/>
	/// <br/>
	/// Requires a restart of WTQ to take effect.
	/// </summary>
	[DefaultValue(true)]
	[Display(Name = "SharpHook hotkeys", Prompt = "Requires restart")]
	public bool? SharpHook { get; set; }
}
