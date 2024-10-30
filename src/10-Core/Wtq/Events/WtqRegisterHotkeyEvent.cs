namespace Wtq.Events;

public sealed class WtqRegisterHotkeyEvent : IWtqEvent
{
	public WtqApp? App { get; init; }

	public Keys Key { get; init; }

	public KeyModifiers Modifiers { get; init; }
}