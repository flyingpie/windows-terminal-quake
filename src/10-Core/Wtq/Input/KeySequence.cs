namespace Wtq.Input;

/// <summary>
/// A combination of a <see cref="KeyModifiers"/>, a <see cref="Keys"/> (KeyCode) and a key character (KeyChar).
/// </summary>
public readonly struct KeySequence(
	KeyModifiers modifiers,
	string? keyChar,
	KeyCode? keyCode)
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
	public KeyCode KeyCode { get; } = keyCode ?? KeyCode.None;

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

	public bool Equals(KeySequence other) =>
		Modifiers == other.Modifiers && KeyCode == other.KeyCode && KeyChar == other.KeyChar;

	public override int GetHashCode() =>
		HashCode.Combine((int)Modifiers, KeyCode, KeyChar);

	public override string ToString() =>
		$"Modifiers:'{Modifiers}' KeyChar:{(HasKeyChar ? $"'{KeyChar}'" : "<none>")} KeyCode:{(HasKeyCode ? $"'{KeyCode}'" : "<none>")}";
}