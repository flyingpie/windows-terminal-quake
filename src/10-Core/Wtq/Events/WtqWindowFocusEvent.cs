namespace Wtq.Events;

public class WtqWindowFocusEvent : IWtqEvent
{
	// public WtqApp? App { get; init; }

	/// <summary>
	/// Window that got focus.
	/// </summary>
	public WtqWindow? GotFocusWindow { get; init; }

	/// <summary>
	/// Window that lost focus.
	/// </summary>
	public WtqWindow? LostFocusWindow { get; init; }

	// public bool GainedFocus { get; init; }
}