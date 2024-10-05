namespace Wtq.Configuration;

/// <summary>
/// Flags that are passed when toggling an app, to differentiate between eg. regular toggles and instant ones.
/// </summary>
public enum ToggleModifiers
{
	/// <summary>
	/// None.
	/// </summary>
	None = 0,

	/// <summary>
	/// Toggle without animating. Useful for setup, such as when WTQ is starting apps.
	/// </summary>
	Instant,

	/// <summary>
	/// Moving from one app to another (usually slightly faster).
	/// </summary>
	SwitchingApps,
}