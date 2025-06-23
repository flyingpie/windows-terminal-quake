namespace Wtq.Configuration;

/// <summary>
/// A combination of a <see cref="KeyModifiers"/>, a <see cref="Keys"/> and a key as a character.
/// </summary>
public readonly struct KeySequence(
	KeyModifiers modifiers,
	string? keyChar,
	Keys? keyCode)
	: IEquatable<KeySequence>
{
	/// <summary>
	/// The optional modifiers (ctrl, shift, etc.).
	/// </summary>
	public KeyModifiers Modifiers { get; } = modifiers;

	[JsonIgnore]
	public bool HasModifiers => Modifiers != KeyModifiers.None;

	/// <summary>
	/// The pressed key (Q, 1, F1, etc.), as a virtual key code.
	/// </summary>
	public Keys KeyCode { get; } = keyCode ?? Keys.None;

	[JsonIgnore]
	public bool HasKeyCode => KeyCode != Keys.None;

	/// <summary>
	/// The pressed key, as a string, as translated by the keyboard layout.
	/// </summary>
	public string? KeyChar { get; } = keyChar;

	[JsonIgnore]
	[MemberNotNullWhen(true, nameof(KeyChar))]
	public bool HasKeyChar => !string.IsNullOrWhiteSpace(KeyChar);

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

	public bool Equals(KeySequence other) =>
		Modifiers == other.Modifiers && KeyCode == other.KeyCode && KeyChar == other.KeyChar;

	public override int GetHashCode() =>
		HashCode.Combine((int)Modifiers, KeyCode, KeyChar);

	public override string ToString() => $"Modifiers:'{Modifiers}' KeyChar:{(HasKeyChar ? $"'{KeyChar}'" : "<none>")} KeyCode:{(HasKeyCode ? $"'{KeyCode}'" : "<none>")}";
}