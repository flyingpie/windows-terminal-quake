namespace Wtq.Configuration;

/// <summary>
/// How WTQ should try to attach to an app.
/// </summary>
public enum AttachMode
{
	/// <summary>
	/// Used to detect serialization issues.
	/// </summary>
	[DisplayFlags(IsVisible = false)] // Don't show this in the GUI, it's purely for internal use.
	None = 0,

	/// <summary>
	/// Only look for existing app instances (but don't create one).
	/// </summary>
	Find,

	/// <summary>
	/// Look for an existing app instance, create one if one does not exist yet.
	/// </summary>
	[Display(Name = "Find or start")]
	FindOrStart,

	/// <summary>
	/// Attach to whatever app is in the foreground when pressing an assigned hotkey.
	/// </summary>
	Manual,
}