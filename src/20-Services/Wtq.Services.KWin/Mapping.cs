using System.Text;
using Wtq.Configuration;
using static Wtq.Configuration.Keys;

namespace Wtq.Services.KWin;

public static class Mapping
{
	public static string ToKWin(this KeySequence sequence)
	{
		return Sequence(sequence);
	}

	public static string Sequence(KeySequence sequence)
	{
		var sb = new StringBuilder();

		// TODO: Determine whether the "Shift" key is required.
		// I.e., with sequence "Shift+F1", it _is_ required, as the actual key isn't affected by the shift.
		// But with "Ctrl+Shift+1", it _is not_ required, as it will be bound as "Ctrl+@" (on US keyboards).

		// Modifier
		if (sequence.Modifiers != KeyModifiers.None)
		{
			sb.Append(ModifierToKWinString(sequence.Modifiers));
		}

		// Key char
		// KWin uses key characters, so prefer that one if we have it right in the settings.
		if (sequence.HasKeyChar)
		{
			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			sb.Append(sequence.KeyChar);
		}

		// Alternatively, if no key character is present, map the key code.
		// This is not a perfect method, as the character depends on the keyboard layout, which I haven't found a way to access yet.
		else if (sequence.HasKeyCode)
		{
			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			sb.Append(KeyToKWinString(sequence.KeyCode));
		}

		Console.WriteLine($"SEQ:{sequence} => KWIN:{sb}");

		return sb.ToString();
	}

	public static string Sequence(KeyModifiers modifiers, Keys key)
	{
		var sb = new StringBuilder();

		if (modifiers != KeyModifiers.None)
		{
			sb.Append(ModifierToKWinString(modifiers));
		}

		if (key != None)
		{
			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			sb.Append(KeyToKWinString(key));
		}

		return sb.ToString();
	}

	private static string KeyToKWinString(Keys key) =>
		key switch
		{
			Oemtilde => "`",

			// F-keys.
			F1 => "F1", F2 => "F2", F3 => "F3", F4 => "F4", F5 => "F5", F6 => "F6", F7 => "F7", F8 => "F8", F9 => "F9", F10 => "F10", F11 => "F11", F12 => "F12", F13 => "F13", F14 => "F14", F15 => "F15", F16 => "F16", F17 => "F17", F18 => "F18", F19 => "F19", F20 => "F20", F21 => "F21", F22 => "F22", F23 => "F23", F24 => "F24",

			// Keys above A-Z keys, under F-keys.
			D0 => "0", D1 => "1", D2 => "2", D3 => "3", D4 => "4", D5 => "5", D6 => "6", D7 => "7", D8 => "8", D9 => "9",

			// Letters.
			A => "A", B => "B", C => "C", D => "D", E => "E", F => "F", G => "G", H => "H", I => "I", J => "J", K => "K", L => "L", M => "M", N => "N", O => "O", P => "P", Q => "Q", R => "R", S => "S", T => "T", U => "U", V => "V", W => "W", X => "X", Y => "Y", Z => "Z",

			// Numpad (can't seem to differentiate from regular numbers?).
			NumPad0 => "0", NumPad1 => "1", NumPad2 => "2", NumPad3 => "3", NumPad4 => "4", NumPad5 => "5", NumPad6 => "6", NumPad7 => "7", NumPad8 => "8", NumPad9 => "9",

			Left => "Left",
			Up => "Up",
			Right => "Right",
			Down => "Down",

			Add => "Num++",
			Back => "Backspace",
			Delete => "Del",
			Divide => "/",
			End => "End",
			Escape => "Esc",
			Home => "Home",
			Insert => "Ins",
			Keys.Decimal => ".",
			Multiply => "*",
			NumLock => "NumLock",
			Pause => "Pause",
			Print => "Print",
			Return => "Return",
			Separator => ".",
			Space => " ",
			Subtract => "-",
			Tab => "Tab",

			OemBackslash => "\\",
			OemCloseBrackets => "[",
			OemMinus => "-",
			OemOpenBrackets => "[",
			OemPeriod => ".",

			// OemPipe => "|",
			OemPipe => "§",
			OemQuestion => "?",
			OemQuotes => "\"",
			OemSemicolon => ";",
			Oemcomma => ",",
			Oemplus => "+",

			VolumeMute => "Volume Mute",
			VolumeDown => "Volume Down",
			VolumeUp => "Volume Up",

			_ => string.Empty,
		};

	private static string ModifierToKWinString(KeyModifiers modifiers)
	{
		var sb = new StringBuilder();

		foreach (var m in (KeyModifiers[])
		[
			KeyModifiers.Control, KeyModifiers.Alt, KeyModifiers.Shift, KeyModifiers.Super, KeyModifiers.Numpad // Order is important. I think.
		])
		{
			if (modifiers.HasFlag(m))
			{
				if (sb.Length > 0)
				{
					sb.Append('+');
				}

				sb.Append(ModifierToKWinString2(m));
			}
		}

		return sb.ToString();
	}

	private static string ModifierToKWinString2(KeyModifiers modifiers)
	{
		return modifiers switch
		{
			KeyModifiers.Control
				=> "Ctrl",
			KeyModifiers.Alt
				=> "Alt",
			KeyModifiers.Shift
				=> "Shift",
			KeyModifiers.Super
				=> "Meta",
			KeyModifiers.Numpad
				=> "Num",
			KeyModifiers.None
				=> string.Empty,
			KeyModifiers.NoRepeat
				=> string.Empty,
			_
				=> string.Empty,
		};
	}
}