using Wtq.Core.Data;
using Wtq.Core.Services;

namespace Wtq.Core.Events;

public sealed class WtqHotKeyPressedEvent : IWtqEvent
{
	public WtqKeys Key { get; set; }

	public WtqKeyModifiers Modifiers { get; set; }

	public WtqActionType ActionType { get; set; }

	public WtqApp? App { get; set; }

	public override string ToString() => $"[{Modifiers}] {Key}";
}