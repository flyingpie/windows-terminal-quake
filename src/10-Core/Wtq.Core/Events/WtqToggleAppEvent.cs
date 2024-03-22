using Wtq.Core.Data;
using Wtq.Core.Services;
using Wtq.Services;

namespace Wtq.Core.Events;

public sealed class WtqToggleAppEvent : IWtqEvent
{
	public WtqActionType ActionType { get; set; }

	public WtqApp? App { get; set; }
}