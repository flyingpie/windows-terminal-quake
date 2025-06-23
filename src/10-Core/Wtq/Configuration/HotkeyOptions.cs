namespace Wtq.Configuration;

/// <summary>
/// Defines a combination of a <see cref="Keys"/> value, with optional <see cref="KeyModifiers"/>, used for mapping a shortcut to an action.
/// </summary>
public sealed class HotkeyOptions
{
	public Keys? Key { get; set; }

	public string? KeyChar { get; set; }

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

	[JsonIgnore]
	public KeySequence Sequence => new(Modifiers, KeyChar, Key);

	public override string ToString() => Sequence.ToString();
}