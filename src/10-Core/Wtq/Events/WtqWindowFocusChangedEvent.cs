namespace Wtq.Events;

/// <summary>
/// Fired when one window loses focus, and/or another one gains focus.<br/>
/// Note that this also includes windows not managed by WTQ.
/// </summary>
public class WtqWindowFocusChangedEvent : WtqEvent
{
	/// <summary>
	/// Window that got focus.
	/// </summary>
	public WtqWindow? GotFocusWindow { get; init; }

	/// <summary>
	/// Window that lost focus.
	/// </summary>
	public WtqWindow? LostFocusWindow { get; init; }
}