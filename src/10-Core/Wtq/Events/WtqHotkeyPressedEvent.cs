namespace Wtq.Events;

public sealed class WtqHotKeyPressedEvent : IWtqEvent
{
	public WtqApp? App { get; init; }

	public Keys Key { get; init; }

	public KeyModifiers Modifiers { get; init; }

	public override string ToString() => $"[{Modifiers}] {Key}";
}