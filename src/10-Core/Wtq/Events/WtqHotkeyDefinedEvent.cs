namespace Wtq.Events;

public sealed class WtqHotkeyDefinedEvent : IWtqEvent
{
	public WtqApp? App { get; init; }

	public Keys Key { get; init; }

	public KeyModifiers Modifiers { get; init; }

	public override string ToString() => $"[{nameof(WtqHotkeyDefinedEvent)}] MOD:{Modifiers} KEY:{Key}";
}