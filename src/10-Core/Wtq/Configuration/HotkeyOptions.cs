namespace Wtq.Configuration;

/// <summary>
/// Defines a combination of a <see cref="Keys"/> value, with optional <see cref="KeyModifiers"/>, used for mapping a shortcut to an action.
/// </summary>
public sealed class HotkeyOptions
{
	/// <summary>
	/// The key to use, as a virtual key code. This is a keyboard-layout-independent code that points to the physical location on the keyboard.
	/// </summary>
	public Keys? Key { get; set; }

	/// <summary>
	/// The key to use, as a character. This is keyboard-layout-dependent, and points to what a key produces, as opposed to where it is on the keyboard.
	/// </summary>
	public string? KeyChar { get; set; }

	/// <summary>
	/// The modifiers that need to be active.
	/// </summary>
	public KeyModifiers Modifiers { get; set; }

	[JsonIgnore]
	public KeySequence Sequence => new(Modifiers, KeyChar, Key);

	public override string ToString() => Sequence.ToString();
}