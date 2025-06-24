namespace Wtq.Events;

/// <summary>
/// Fired when a hotkey has been configured, on app start and config changes.
/// </summary>
public sealed class WtqHotkeyDefinedEvent : WtqEvent
{
	/// <summary>
	/// The app with which the hotkey is associated. Can be null, for the "most recent app"-hotkey.
	/// </summary>
	public WtqAppOptions? AppOptions { get; init; }

	/// <summary>
	/// The key sequence that was specified.
	/// </summary>
	public KeySequence Sequence { get; init; }
}