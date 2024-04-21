using Wtq.Core.Data;
using Wtq.Core.Services;

namespace Wtq.Core.Events;

public class WtqAppFocusEvent : IWtqEvent
{
	public WtqActionType ActionType { get; set; }

	public WtqApp? App { get; set; }

	public bool GainedFocus { get; set; }
}