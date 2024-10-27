namespace Wtq.Events;

public sealed class WtqAppToggledEvent : WtqEvent
{
	public required WtqApp App { get; init; }
}