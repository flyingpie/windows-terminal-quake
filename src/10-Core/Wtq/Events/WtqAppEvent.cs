namespace Wtq.Events;

public abstract class WtqAppEvent : WtqEvent
{
	/// <summary>
	/// Name of the app to toggle.
	/// </summary>
	public required string AppName { get; init; }
}