using Wtq.Core.Services;

namespace Wtq.Core.Data;

public class WtqAction
{
	public WtqActionType ActionType { get; set; }

	public WtqApp? App { get; set; }
}