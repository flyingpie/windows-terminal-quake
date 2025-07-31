namespace Wtq.Configuration;

/// <summary>
/// Style of the tray icon (i.e. light or dark).
/// </summary>
public enum TrayIconStyle
{
	/// <summary>
	/// Used to detect serialization issues.
	/// </summary>
	[DisplayFlags(IsVisible = false)] // Don't show this in the GUI, it's purely for internal use.
	None = 0,

	/// <summary>
	/// Attempt to detect the OS theme and use the appropriate style based on that.
	/// </summary>
	Auto,

	/// <summary>
	/// Dark icon, works best on lighter themes.
	/// </summary>
	Dark,

	/// <summary>
	/// Light icon, works best on darker themes.
	/// </summary>
	Light,
}