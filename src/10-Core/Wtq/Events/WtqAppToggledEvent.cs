namespace Wtq.Events;

/// <summary>
/// Fired when an app is requested to toggle (i.e. hotkey is pressed).<br/>
/// This is one layer above <see cref="WtqHotkeyPressedEvent"/>, in that it has the <see cref="WtqApp"/> to toggle included.
/// </summary>
public sealed class WtqAppToggledEvent(string appName)
	: WtqAppEvent(appName)
{
}