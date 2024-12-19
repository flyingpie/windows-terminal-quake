namespace Wtq.Configuration;

/// <summary>
/// Whether to toggle off an app when focus moves to another app.
/// </summary>
[SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "MvdO: Backward compat.")]
public enum HideOnFocusLost
{
	/// <summary>
	/// For detecting serialization issues.
	/// </summary>
	None = 0,

	/// <summary>
	/// Toggle off the app when focus is lost.
	/// </summary>
	[Display(Description = "Always")]
	Always = 1,

	/// <summary>
	/// Same as <see cref="Always"/>, here for backward compat from when <see cref="WtqAppOptions.HideOnFocusLost"/> was a boolean.
	/// </summary>
	True = 1,

	/// <summary>
	/// Do not toggle off the app when focus is lost.
	/// </summary>
	[Display(Description = "Never")]
	Never = 2,

	/// <summary>
	/// Same as <see cref="Never"/>, here for backward compat from when <see cref="WtqAppOptions.HideOnFocusLost"/> was a boolean.
	/// </summary>
	False = 2,

	/// <summary>
	/// Only toggle off the app when focus went to an app on a different screen.
	/// </summary>
	[Display(Description = "Unless focus changed screen")]
	UnlessFocusChangedScreen = 3,
}