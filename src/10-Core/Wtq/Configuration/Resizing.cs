namespace Wtq.Configuration;

/// <summary>
/// Whether- and when to resize app windows.
/// </summary>
public enum Resizing
{
	/// <summary>
	/// Used for detecting serialization issues.
	/// </summary>
	[DisplayFlags(IsVisible = false)] // Don't show this in the GUI, it's purely for internal use.
	None = 0,

	/// <summary>
	/// Always resize the app window to match the alignment settings (like <see cref="WtqSharedOptions.HorizontalScreenCoverage"/>).
	/// </summary>
	Always,

	/// <summary>
	/// Never resize the app window, ignoring aligment settings (like <see cref="WtqSharedOptions.HorizontalScreenCoverage"/>).
	/// </summary>
	Never,
}