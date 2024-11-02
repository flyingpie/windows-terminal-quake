namespace Wtq.Configuration;

/// <summary>
/// From what direction an app window should move onto the screen.
/// </summary>
public enum ToggleDirection
{
	/// <summary>
	/// For serialization issue checking.
	/// </summary>
	None = 0,

	/// <summary>
	/// Move above the screen.
	/// </summary>
	Up,

	/// <summary>
	/// Move below the screen.
	/// </summary>
	Down,

	/// <summary>
	/// Move left of the screen.
	/// </summary>
	Left,

	/// <summary>
	/// Move right of the screen.
	/// </summary>
	Right,
}