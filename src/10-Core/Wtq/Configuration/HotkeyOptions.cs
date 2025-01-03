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

	public override string ToString() => $"{Modifiers} + {Key}";
}