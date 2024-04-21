namespace Wtq.Events;

public sealed class WtqHotKeyPressedEvent : IWtqEvent
{
	public WtqApp? App { get; set; }

	public Keys Key { get; set; }

	public KeyModifiers Modifiers { get; set; }

	public override string ToString() => $"[{Modifiers}] {Key}";
}