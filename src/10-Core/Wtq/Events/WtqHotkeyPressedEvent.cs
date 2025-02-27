using Wtq.Services;

namespace Wtq.Events;

/// <summary>
/// Fired when a hotkey is pressed. Does not include the (optionally) related app.<br/>
/// Note that this event is used for OS-specific hotkey-handling.<br/>
/// The non-platform-specific <see cref="WtqHotkeyRoutingService"/> catches these, and fires new events that include the relevant app.
/// </summary>
public sealed class WtqHotkeyPressedEvent(KeyModifiers mod, Keys key) : WtqEvent
{
	/// <summary>
	/// The pressed key (Q, 1, F1, etc.).
	/// </summary>
	public Keys Key { get; } = key;

	/// <summary>
	/// The optional modifiers (ctrl, shift, etc.).
	/// </summary>
	public KeyModifiers Modifiers { get; } = mod;
}