namespace Wtq.Configuration;

/// <summary>
/// Where app windows should be moved to- and from, relative to the preferred screen.
/// </summary>
public enum OffScreenLocation
{
	None,

	/// <summary>
	/// Above the screen.
	/// </summary>
	Above,

	/// <summary>
	/// Below the screen.
	/// </summary>
	Below,

	Left,

	Right,

	Void,
}