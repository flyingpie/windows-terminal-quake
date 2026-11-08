namespace Wtq.Configuration;

/// <summary>
/// Determines where to place an app window on the screen, vertically.
/// </summary>
public enum VerticalAlign
{
	/// <summary>
	/// Center of the screen.
	/// </summary>
	Center = 0,

	/// <summary>
	/// Top of the screen.
	/// </summary>
	Top,

	/// <summary>
	/// Bottom of the screen.
	/// </summary>
	Bottom,

	/// <summary>
	/// Keep the vertical position as-is, don't align it.
	/// </summary>
	None,
}
