namespace Wtq.Events;

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