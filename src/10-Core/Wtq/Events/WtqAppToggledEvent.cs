namespace Wtq.Events;

public sealed class WtqAppToggledEvent : IWtqEvent
{
	public WtqApp? App { get; init; }
}