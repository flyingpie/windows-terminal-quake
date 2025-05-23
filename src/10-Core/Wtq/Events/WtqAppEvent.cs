namespace Wtq.Events;

/// <summary>
/// An event that involves an app.
/// </summary>
public abstract class WtqAppEvent(string appName) : WtqEvent
{
	/// <summary>
	/// Name of the app that relates to this event.
	/// </summary>
	public string AppName { get; } = Guard.Against.NullOrWhiteSpace(appName);
}