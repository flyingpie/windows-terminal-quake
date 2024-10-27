namespace Wtq.Events;

public sealed class WtqUpdateEvent : WtqEvent
{
	public TimeSpan Elapsed { get; set; }
}