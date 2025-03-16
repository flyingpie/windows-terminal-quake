namespace Wtq.Configuration;

/// <summary>
/// Determines the visibility of the app's taskbar icon.
/// </summary>
public enum TaskbarIconVisibility
{
	/// <summary>
	/// Used to detect serialization issues.
	/// </summary>
	[DisplayFlags(IsVisible = false)] // Don't show this in the GUI, it's purely for internal use.
	None = 0,

	/// <summary>
	/// <strong>Never</strong> show the taskbar icon.
	/// </summary>
	[Display(Name = "Always hidden")]
	AlwaysHidden,

	/// <summary>
	/// <strong>Always</strong> show the taskbar icon (note that this can look a bit weird when the app is toggled off).
	/// </summary>
	[Display(Name = "Always visible")]
	AlwaysVisible,

	/// <summary>
	/// Only show the taskbar icon when the app is toggled <strong>on</strong>.
	/// </summary>
	[Display(Name = "When the app is visible")]
	WhenAppVisible,
}