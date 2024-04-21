namespace Wtq.Events;

public sealed class WtqRegisterHotKeyEvent : IWtqEvent
{
	public WtqApp? App { get; set; }

	public Keys Key { get; set; }

	public KeyModifiers Modifiers { get; set; }

	public override string ToString() => $"[{nameof(WtqRegisterHotKeyEvent)}] MOD:{Modifiers} KEY:{Key}";
}