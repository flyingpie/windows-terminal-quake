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
	/// Toggle <strong>off</strong> the app when focus is lost.
	/// </summary>
	[Display(Name = "Always")]
	Always = 1,

	/// <summary>
	/// Same as <see cref="Always"/>, here for backward compat from when <see cref="WtqAppOptions.HideOnFocusLost"/> was a boolean.
	/// </summary>
	[DisplayFlags(IsVisible = false)]
	True = 1001, // TODO: Use explicitly

	/// <summary>
	/// Do not toggle off the app when focus is lost.
	/// </summary>
	[Display(Name = "Never")]
	Never = 2,

	/// <summary>
	/// Same as <see cref="Never"/>, here for backward compat from when <see cref="WtqAppOptions.HideOnFocusLost"/> was a boolean.
	/// </summary>
	[DisplayFlags(IsVisible = false)]
	False = 1002, // TODO: Use explicitly

	/// <summary>
	/// Only toggle off the app when focus went to an app on a different screen.
	/// </summary>
	[Display(Name = "Unless focus changed screen")]
	UnlessFocusChangedScreen = 3,
}