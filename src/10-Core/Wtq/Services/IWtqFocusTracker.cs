namespace Wtq.Services;

public interface IWtqFocusTracker
{
	WtqApp? ForegroundApp { get; }

	WtqWindow? LastNonWtqForeground { get; }
}