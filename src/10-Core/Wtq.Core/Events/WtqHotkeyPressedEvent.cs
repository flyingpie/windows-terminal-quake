using Wtq.Core.Data;
using Wtq.Core.Services;
using Wtq.Services;

namespace Wtq.Core.Events;

public sealed class WtqHotkeyPressedEvent : IWtqEvent
{
	public WtqKeys Key { get; set; }

	public WtqKeyModifiers Modifiers { get; set; }

	public WtqActionType ActionType { get; set; }

	public WtqApp? App { get; set; }

	public override string ToString() => $"[{Modifiers}] {Key}";
}