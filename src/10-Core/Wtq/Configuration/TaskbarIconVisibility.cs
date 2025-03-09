namespace Wtq.Configuration;

/// <summary>
/// Determines the visibility of the app's taskbar icon.
/// </summary>
public enum TaskbarIconVisibility
{
	None = 0,

	/// <summary>
	/// <strong>Never</strong> show the task bar icon.
	/// </summary>
	[Display(Name = "Always hidden")]
	AlwaysHidden,

	/// <summary>
	/// <strong>Always</strong> show the task bar icon (note that this can look a bit weird when the app is toggled off).
	/// </summary>
	[Display(Name = "Always visible")]
	AlwaysVisible,

	/// <summary>
	/// Only show the task bar icon when the app is toggled <strong>on</strong>.
	/// </summary>
	[Display(Name = "When the app is visible")]
	WhenAppVisible,
}