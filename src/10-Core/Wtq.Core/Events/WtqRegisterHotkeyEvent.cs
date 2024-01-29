using Wtq.Core.Data;
using Wtq.Core.Services;
using Wtq.Services;

namespace Wtq.Core.Events;

public sealed class WtqRegisterHotkeyEvent : IWtqEvent
{
	public WtqActionType ActionType { get; set; }

	public WtqApp? App { get; set; }

	public WtqKeyModifiers Modifiers { get; set; }

	public WtqKeys Key { get; set; }

	public override string ToString() => $"[RegisterHotKey] MOD:{Modifiers} KEY:{Key}";
}