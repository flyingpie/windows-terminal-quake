namespace Wtq.Configuration;

/// <summary>
/// What monitor an app should be toggle on.
/// </summary>
public enum PreferMonitor
{
	/// <summary>
	/// The monitor where the mouse cursor is currently at.
	/// </summary>
	[Display(Name = "With cursor")]
	WithCursor = 0,

	/// <summary>
	/// The monitor at the index as specified by <strong>MonitorIndex</strong> (0-based).
	/// </summary>
	[Display(Name = "At index")]
	AtIndex,

	/// <summary>
	/// The monitor considered <strong>primary</strong> by the OS.
	/// </summary>
	Primary,
}