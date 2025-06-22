namespace Wtq.Configuration;

/// <summary>
/// A combination of a <see cref="KeyModifiers"/>, a <see cref="Keys"/> and a key as a character.
/// </summary>
public readonly struct KeySequence(KeyModifiers modifiers, string? keyChar, Keys? keyCode)
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

	// TODO: Use Keys values (e.g. "Control" instead of "Ctrl", this is taken from the kwin serializer.
	public override string ToString()
	{
		var s = new StringBuilder();

		if (Modifiers.HasFlag(KeyModifiers.Super))
		{
			if (s.Length > 0)
			{
				s.Append(" + ");
			}

			s.Append("Super");
		}

		if (Modifiers.HasFlag(KeyModifiers.Control))
		{
			if (s.Length > 0)
			{
				s.Append(" + ");
			}

			s.Append("Ctrl");
		}

		if (Modifiers.HasFlag(KeyModifiers.Shift))
		{
			if (s.Length > 0)
			{
				s.Append(" + ");
			}

			s.Append("Shift");
		}

		if (Modifiers.HasFlag(KeyModifiers.Alt))
		{
			if (s.Length > 0)
			{
				s.Append(" + ");
			}

			s.Append("Alt");
		}

		if (Modifiers.HasFlag(KeyModifiers.Numpad))
		{
			if (s.Length > 0)
			{
				s.Append(" + ");
			}

			s.Append("Num");
		}

		if (HasKeyChar)
		{
			if (s.Length > 0)
			{
				s.Append(" + ");
			}

			s.Append(KeyChar);
		}
		else if (HasKeyCode)
		{
			if (s.Length > 0)
			{
				s.Append(" + ");
			}

			s.Append(KeyCode.GetAttribute<Keys, DisplayAttribute>()?.Description ?? KeyCode.ToString());
		}

		return s.ToString();
	}

	public string ToLongString() => $"Modifiers:'{Modifiers}' KeyChar:{(HasKeyChar ? $"'{KeyChar}'" : "<none>")} KeyCode:{(HasKeyCode ? $"'{KeyCode}'" : "<none>")}";
}