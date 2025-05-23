namespace Wtq.Configuration;

/// <summary>
/// Defines a combination of a <see cref="Keys"/> value, with optional <see cref="KeyModifiers"/>, used for mapping a shortcut to an action.
/// </summary>
public sealed class HotkeyOptions
{
	public Keys Key { get; set; }

	public KeyModifiers Modifiers { get; set; }

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

		s.Append(Key.GetAttribute<Keys, DisplayAttribute>()?.Description ?? Key.ToString());

		return s.ToString();
	}
}