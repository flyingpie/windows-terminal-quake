namespace Wtq.Configuration;

/// <summary>
/// How WTQ should try to attach to an app.
/// </summary>
public enum AttachMode
{
	/// <summary>
	/// Used to detect serialization issues.
	/// </summary>
	None = 0,

	/// <summary>
	/// Only look for existing app instances.
	/// </summary>
	Find,

	/// <summary>
	/// Look for an existing app instance, create one of one does not exist yet.
	/// </summary>
	FindOrStart,

	/// <summary>
	/// Attach to the foreground app when pressing the assigned hot key.
	/// </summary>
	Manual,
}