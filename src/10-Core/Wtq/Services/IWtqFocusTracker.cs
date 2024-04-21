namespace Wtq.Services;

public interface IWtqFocusTracker
{
	WtqApp? ForegroundApp { get; }

	uint? LastNonWtqForeground { get; }
}