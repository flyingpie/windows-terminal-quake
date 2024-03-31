using Wtq.Core.Data;

namespace Wtq.Core.Services;

public interface IWtqEvent
{
	WtqActionType ActionType { get; }

	WtqApp? App { get; }
}