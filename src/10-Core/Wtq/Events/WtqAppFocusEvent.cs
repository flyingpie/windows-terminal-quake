namespace Wtq.Events;

public class WtqAppFocusEvent : IWtqEvent
{
	public WtqApp? App { get; set; }

	public bool GainedFocus { get; set; }
}