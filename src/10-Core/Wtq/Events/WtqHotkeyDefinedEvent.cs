namespace Wtq.Events;

public sealed class WtqHotkeyDefinedEvent : WtqEvent
{
	public WtqApp? App { get; init; }

	public required Keys Key { get; init; }

	public required KeyModifiers Modifiers { get; init; }
}