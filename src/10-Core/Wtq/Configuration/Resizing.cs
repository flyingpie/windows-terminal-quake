namespace Wtq.Configuration;

public enum Resizing
{
	/// <summary>
	/// Used for detecting serialization issues.
	/// </summary>
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