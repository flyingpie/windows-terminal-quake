namespace Wtq.Utils;

/// <summary>
/// Represents the theme of the OS (light or dark).<br/>
/// Used to determine (things like) the tray icon color.
/// </summary>
public enum OsColorMode
{
	/// <summary>
	/// Used to detect serialization issues.
	/// </summary>
	[DisplayFlags(IsVisible = false)] // Don't show this in the GUI, it's purely for internal use.
	None = 0,

	/// <summary>
	/// Dark mode.
	/// </summary>
	Dark,

	/// <summary>
	/// Light mode.
	/// </summary>
	Light,

	/// <summary>
	/// Couldn't determine the color mode.
	/// </summary>
	Unknown,
}