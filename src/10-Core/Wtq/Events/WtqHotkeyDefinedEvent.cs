namespace Wtq.Events;

/// <summary>
/// Fired when a hotkey has been configured, on app start and config changes.
/// </summary>
public sealed class WtqHotkeyDefinedEvent : WtqEvent
{
	/// <summary>
	/// The app with which the hotkey is associated. Can be null, for the "most recent app"-hotkey.
	/// </summary>
	public WtqApp? App { get; init; }

	/// <summary>
	/// The pressed key (Q, 1, F1, etc.).
	/// </summary>
	public required Keys Key { get; init; }

	/// <summary>
	/// The optional modifiers (ctrl, shift, etc.).
	/// </summary>
	public required KeyModifiers Modifiers { get; init; }
}