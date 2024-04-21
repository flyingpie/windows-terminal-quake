namespace Wtq.Core.Data;

public enum ToggleModifiers
{
	/// <summary>
	/// None.
	/// </summary>
	None = 0,

	/// <summary>
	/// Toggle without animating.
	/// </summary>
	Instant,

	/// <summary>
	/// Moving from one app to another (usually slightly faster).
	/// </summary>
	SwitchingApps,
}