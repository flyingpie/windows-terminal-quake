namespace Wtq.Configuration;

/// <summary>
/// Defines a combination of a <see cref="Keys"/> value, with optional <see cref="KeyModifiers"/>, used for mapping a shortcut to an action.
/// </summary>
public sealed class HotkeyOptions
{
	private Keys? _keyCode;

	public Keys? Key {get;set;}
	// {
	// 	get => HasKeyChar ? null : _keyCode; // Only serialize this property if no "KeyChar" is present.
	// 	set => _keyCode = value;
	// }

	public string? KeyChar { get; set; }

	public KeyModifiers Modifiers { get; set; }

	[JsonIgnore]
	public bool HasKeyChar => !string.IsNullOrWhiteSpace(KeyChar);

	[JsonIgnore]
	public bool HasKeyCode => Key != Keys.None;

	[JsonIgnore]
	public bool IsAlt
	{
		get => Modifiers.HasFlag(KeyModifiers.Alt);
		set
		{
			if (value)
			{
				Modifiers |= KeyModifiers.Alt;
			}
			else
			{
				Modifiers &= ~KeyModifiers.Alt;
			}
		}
	}

	[JsonIgnore]
	public bool IsCtrl
	{
		get => Modifiers.HasFlag(KeyModifiers.Control);
		set
		{
			if (value)
			{
				Modifiers |= KeyModifiers.Control;
			}
			else
			{
				Modifiers &= ~KeyModifiers.Control;
			}
		}
	}

	[JsonIgnore]
	public bool IsShift
	{
		get => Modifiers.HasFlag(KeyModifiers.Shift);
		set
		{
			if (value)
			{
				Modifiers |= KeyModifiers.Shift;
			}
			else
			{
				Modifiers &= ~KeyModifiers.Shift;
			}
		}
	}

	[JsonIgnore]
	public bool IsSuper
	{
		get => Modifiers.HasFlag(KeyModifiers.Super);
		set
		{
			if (value)
			{
				Modifiers |= KeyModifiers.Super;
			}
			else
			{
				Modifiers &= ~KeyModifiers.Super;
			}
		}
	}

	/// <summary>
	/// Whether both the modifier and the key are set to "None".
	/// </summary>
	[JsonIgnore]
	public bool IsEmpty => Modifiers == KeyModifiers.None && Key == Keys.None;

	[JsonIgnore]
	public KeySequence Sequence => new()
	{
		KeyChar = KeyChar,
		KeyCode = Key,
		Modifiers = Modifiers,
	};

	public override string ToString()
	{
		var s = new StringBuilder();

		if (IsCtrl)
		{
			s.Append("Ctrl");
		}

		if (IsShift)
		{
			if (s.Length > 0)
			{
				s.Append(" + ");
			}

			s.Append("Shift");
		}

		if (IsAlt)
		{
			if (s.Length > 0)
			{
				s.Append(" + ");
			}

			s.Append("Alt");
		}

		if (IsSuper)
		{
			if (s.Length > 0)
			{
				s.Append(" + ");
			}

			s.Append("Super");
		}

		if (s.Length > 0)
		{
			s.Append(" + ");
		}

		if (HasKeyChar)
		{
			s.Append(KeyChar);
		}

		if (Key.HasValue && Key != Keys.None)
		{
			s.Append(Key.Value.GetAttribute<Keys, DisplayAttribute>()?.Description ?? Key.ToString());
		}

		return s.ToString();
	}
}