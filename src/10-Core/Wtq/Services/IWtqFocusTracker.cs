namespace Wtq.Services;

public interface IWtqFocusTracker
{
	/// <summary>
	/// Bring focus to the most recent app that is not controlled by WTQ.<br/>
	/// Useful for when a WTQ-controlled app is toggled out, to make sure any input events
	/// are sent to whatever is now on top.<br/>
	/// Doesn't do anything if no non-WTQ app is known by WTQ.
	/// </summary>
	Task FocusLastNonWtqAppAsync();
}