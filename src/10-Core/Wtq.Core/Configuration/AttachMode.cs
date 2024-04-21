namespace Wtq.Core.Configuration;

/// <summary>
/// How WTQ should try to attach to a process.
/// </summary>
public enum AttachMode
{
	/// <summary>
	/// Used to detect serialization issues.
	/// </summary>
	None = 0,

	/// <summary>
	/// Always create a new process.
	/// </summary>
	Start,

	/// <summary>
	/// Only look for existing process.
	/// </summary>
	Find,

	/// <summary>
	/// Look for existing process, create one of one does not exist yet.
	/// </summary>
	FindOrStart,

	/// <summary>
	/// Attach to the foreground process when pressing the assigned hot key.
	/// </summary>
	Manual,
}