namespace Wtq.Events;

/// <summary>
/// An event that involves an app.
/// </summary>
public abstract class WtqAppEvent : WtqEvent
{
	/// <summary>
	/// Name of the app to toggle.
	/// </summary>
	public required string AppName { get; init; }
}