namespace Wtq.Events;

public sealed class WtqHotkeyPressedEvent : WtqEvent
{
	public WtqApp? App { get; init; }

	public required Keys Key { get; init; }

	public required KeyModifiers Modifiers { get; init; }

	public override string ToString() => $"[{Modifiers}] {Key}";
}