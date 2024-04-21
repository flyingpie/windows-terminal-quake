namespace Wtq.Events;

public sealed class WtqToggleAppEvent : IWtqEvent
{
	public WtqApp? App { get; set; }
}