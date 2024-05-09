namespace Wtq.Services;

public interface IWtqFocusTracker
{
	// TODO: Remove after refactoring to event.
	WtqWindow? LastNonWtqForeground { get; }
}