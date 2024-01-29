using Wtq.Core.Data;
using Wtq.Services;

namespace Wtq.Core.Services;

public interface IWtqEvent
{
	WtqActionType ActionType { get; }

	WtqApp? App { get; }
}
