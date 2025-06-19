namespace Wtq.Configuration;

/// <summary>
/// A combination of a <see cref="KeyModifiers"/>, a <see cref="Keys"/> and a key as a character.
/// </summary>
public readonly struct KeySequence(KeyModifiers modifiers, Keys? keyCode, string? keyChar)
	: IEquatable<KeySequence>
{
	/// <summary>
	/// The optional modifiers (ctrl, shift, etc.).
	/// </summary>
	public KeyModifiers Modifiers { get; } = modifiers;

	/// <summary>
	/// The pressed key (Q, 1, F1, etc.), as a virtual key code.
	/// </summary>
	public Keys? KeyCode { get; } = keyCode;

	/// <summary>
	/// The pressed key, as a string, as translated by the keyboard layout.
	/// </summary>
	public string? KeyChar { get; } = keyChar;

	public static bool operator ==(KeySequence left, KeySequence right) =>
		left.Equals(right);

	public static bool operator !=(KeySequence left, KeySequence right) =>
		!(left == right);

	public override readonly bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is KeySequence s)
		{
			return Equals(s);
		}

		return false;
	}

	public readonly bool Equals(KeySequence other) =>
		Modifiers == other.Modifiers && KeyCode == other.KeyCode && KeyChar == other.KeyChar;

	public override readonly int GetHashCode() =>
		HashCode.Combine((int)Modifiers, KeyCode, KeyChar);
}