namespace Wtq.Input;

/// <summary>
/// A combination of a <see cref="KeyModifiers"/>, a <see cref="Keys"/> (KeyCode) and a key character (KeyChar).
/// </summary>
public readonly struct KeySequence(
	KeyModifiers modifiers,
	string? keyChar,
	KeyCode keyCode)
	: IEquatable<KeySequence>
{
	/// <summary>
	/// The optional modifiers (ctrl, shift, alt, super, numpad).
	/// </summary>
	public KeyModifiers Modifiers { get; } = modifiers;

	/// <summary>
	/// Returns whether <see cref="Modifiers"/> has at least 1 modifier set.
	/// </summary>
	[JsonIgnore]
	public bool HasModifiers => Modifiers != KeyModifiers.None;

	/// <summary>
	/// The pressed key (Q, 1, F1, etc.), as a virtual key code.
	/// </summary>
	public KeyCode KeyCode { get; } = keyCode;

	/// <summary>
	/// Returns whether <see cref="KeyCode"/> is not empty.
	/// </summary>
	[JsonIgnore]
	public bool HasKeyCode => KeyCode != KeyCode.None;

	/// <summary>
	/// The pressed key, as a string, as translated by the keyboard layout.
	/// </summary>
	public string? KeyChar { get; } = keyChar;

	/// <summary>
	/// Returns whether <see cref="KeyChar"/> is not empty.
	/// </summary>
	[JsonIgnore]
	[MemberNotNullWhen(true, nameof(KeyChar))]
	public bool HasKeyChar => !string.IsNullOrWhiteSpace(KeyChar);

	/// <summary>
	/// Returns whether at least one of <see cref="Modifiers"/>, <see cref="KeyChar"/> and <see cref="KeyCode"/> has a value.
	/// </summary>
	[JsonIgnore]
	public bool IsEmpty => !HasModifiers && !HasKeyChar && !HasKeyCode;

	public static bool operator ==(KeySequence left, KeySequence right) =>
		left.Equals(right);

	public static bool operator !=(KeySequence left, KeySequence right) =>
		!(left == right);

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is KeySequence s)
		{
			return Equals(s);
		}

		return false;
	}

	public bool Equals(KeySequence other)
	{
		// Modifiers have to be the same.
		if (Modifiers != other.Modifiers)
		{
			return false;
		}

		// If a key character was present on both sides, and it matches, the key sequence can be considered the same.
		if (HasKeyChar && other.HasKeyChar && KeyChar.Equals(other.KeyChar, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		// Alternatively, if a virtual key code was present on both sides, and it matches, also good.
		if (HasKeyCode && other.HasKeyCode && KeyCode == other.KeyCode)
		{
			return true;
		}

		// If we get here, neither the key char nor the key code matched.
		// Note that a key is required, we can't have a key sequence with only a modifier.
		return false;
	}

	public override int GetHashCode() =>
		HashCode.Combine((int)Modifiers, KeyCode, KeyChar);

	public override string ToString() =>
		$"Modifiers:'{Modifiers}' KeyChar:{(HasKeyChar ? $"'{KeyChar}'" : "<none>")} KeyCode:{(HasKeyCode ? $"'{KeyCode}'" : "<none>")}";

	/// <summary>
	/// Returns whether the "SHIFT"-modifier has caused the pressed key to emit a symbol, unrelated to the one that
	/// would have been emitted, had shift not been pressed.<br/>
	/// <br/>
	/// For example: when pressing the "A" key without shift returns "a", with shift "A". These are different, but related.<br/>
	/// Pressing the "1" key on the main row of a US ANSI keyboard without shift returns "1", with shift "!". These are different, and not related.<br/>
	/// <br/>
	/// This is not a perfect method, but we need it for sending hotkey registrations to KWin, as there, "SHIFT" is not considered
	/// when the character already implies one.<br/>
	/// In other words, sending "CTRL+SHIFT+!" to KWin, does not work, it has to be sent as "CTRL+!".<br/>
	/// <br/>
	/// Nicer methods would probably require more access to the active keyboard layout and character mapping, which we don't have.
	/// </summary>
	public bool IsShiftImplied()
	{
		// The "shift" key must be part of the active modifiers.
		if (!Modifiers.HasShift())
		{
			return false;
		}

		// We can skip non-character keys.
		if (KeyChar == null)
		{
			return false;
		}

		// Don't consider key chars that are referring to a non-character key, like "Tab", or "F1".
		if (KeyChar.Length > 1)
		{
			return false;
		}

		return KeyChar.All(c => !char.IsUpper(c) && !char.IsLower(c));
	}

	public string ToShortString()
	{
		var sb = new StringBuilder();

		// Modifier
		foreach (var m in new[] { KeyModifiers.Control, KeyModifiers.Alt, KeyModifiers.Shift, KeyModifiers.Super, KeyModifiers.Numpad, })
		{
			if (!Modifiers.HasFlag(m))
			{
				continue;
			}

			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			sb.Append(m);
		}

		// Key char
		// KWin uses key characters, so prefer that one if we have it right in the settings.
		if (HasKeyChar)
		{
			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			sb.Append(KeyChar);
		}

		// Alternatively, if no key character is present, map the key code.
		// This is not a perfect method, as the character depends on the keyboard layout, which I haven't found a way to access yet.
		else if (HasKeyCode)
		{
			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			sb.Append(KeyCode);
		}

		return sb.ToString();
	}
}