namespace Wtq.Events;

/// <summary>
/// Fired when an app is requested to toggle (i.e. hotkey is pressed).<br/>
/// This is one layer above <see cref="WtqHotkeyPressedEvent"/>, in that it has the <see cref="WtqApp"/> to toggle included.
/// </summary>
public sealed class WtqAppToggledEvent : WtqEvent
{
	/// <summary>
	/// The app to toggle.
	/// </summary>
	public WtqApp App { get; set; }

	public string AppName { get; set; }
}