namespace Wtq.Events;

public class WtqAppFocusEvent : IWtqEvent
{
	public WtqApp? App { get; init; }

	public bool GainedFocus { get; init; }
}