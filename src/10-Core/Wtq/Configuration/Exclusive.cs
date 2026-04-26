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
	/// App will be the only one on-screen at the same time.
	/// </summary>
	[Display(Name = "Always")]
	Always,

	/// <summary>
	/// App can co-exist with other non-exclusive apps.
	/// </summary>
	[Display(Name = "Never")]
	Never,
}