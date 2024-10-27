namespace Wtq.Events;

public sealed class WtqAppToggledEvent : WtqEvent
{
	public WtqApp? App { get; init; }
}