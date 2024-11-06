namespace Wtq.Configuration;

/// <summary>
/// Where app windows should be moved to- and from, relative to the preferred screen.
/// </summary>
public enum OffScreenLocation
{
	/// <summary>
	/// Used for detecting serialization issues.
	/// </summary>
	None = 0,

	/// <summary>
	/// Above the screen.
	/// </summary>
	Above,

	/// <summary>
	/// Below the screen.
	/// </summary>
	Below,

	/// <summary>
	/// Left of the screen.
	/// </summary>
	Left,

	/// <summary>
	/// Right of the screen.
	/// </summary>
	Right,
}