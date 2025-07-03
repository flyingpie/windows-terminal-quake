namespace Wtq.Configuration;

/// <summary>
/// How WTQ should start an app.
/// </summary>
public enum AutoStartMode
{
	/// <summary>
	/// Used to detect serialization issues.
	/// </summary>
	[DisplayFlags(IsVisible = false)] // Don't show this in the GUI, it's purely for internal use.
	None = 0,

	/// <summary>
	/// Start app as soon as WTQ starts.
	/// </summary>
	[Display(Name = "On WTQ start")]
	OnWtqStart,

	/// <summary>
	/// Start app on hotkey press, and it's not running yet.
	/// </summary>
	[Display(Name = "On hotkey press")]
	OnHotkeyPress,
}