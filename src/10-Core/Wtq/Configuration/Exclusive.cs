namespace Wtq.Configuration;

/// <summary>
/// Whether/when an app should exclusively be active.<br/>
/// E.g., when "Always", an app must be the only one of the configured apps to be on the screen.<br/>
/// When "Never", other apps can remain on-screen.
/// </summary>
[SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "MvdO: Backward compat.")]
public enum Exclusive
{
	/// <summary>
	/// For detecting serialization issues.
	/// </summary>
	[DisplayFlags(IsVisible = false)] // Don't show this in the GUI, it's purely for internal use.
	None = 0,

	/// <summary>
	/// Toggle off the app when focus is lost.
	/// </summary>
	[Display(Name = "Always")]
	Always,

	/// <summary>
	/// Do not toggle off the app when focus is lost.
	/// </summary>
	[Display(Name = "Never")]
	Never,

	///// <summary>
	///// Only toggle off the app when focus went to an app on a different screen.
	///// </summary>
	//[Display(Name = "Unless focus changed screen")]
	//WithinScreen,
}