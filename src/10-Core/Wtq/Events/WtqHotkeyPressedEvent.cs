using Wtq.Services;

namespace Wtq.Events;

/// <summary>
/// Fired when a hotkey is pressed. Does not include the (optionally) related app.<br/>
/// Note that this event is used for OS-specific hotkey-handling.<br/>
/// The non-platform-specific <see cref="WtqHotkeyRoutingService"/> catches these, and fires new events that include the relevant app.
/// </summary>
public sealed class WtqHotkeyPressedEvent(KeySequence sequence) : WtqEvent
{
	public KeySequence Sequence { get; } = sequence;
}