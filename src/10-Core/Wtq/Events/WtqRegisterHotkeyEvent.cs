namespace Wtq.Events;

public sealed class WtqRegisterHotKeyEvent : IWtqEvent
{
	public WtqApp? App { get; init; }

	public Keys Key { get; init; }

	public KeyModifiers Modifiers { get; init; }

	public override string ToString() => $"[{nameof(WtqRegisterHotKeyEvent)}] MOD:{Modifiers} KEY:{Key}";
}