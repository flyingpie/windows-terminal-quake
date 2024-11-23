using Wtq.Configuration;
using static Wtq.Configuration.Keys;

namespace Wtq.Services.KWin;

public static class Mapping
{
	public static string Sequence(KeyModifiers modifiers, Keys key)
	{
		var kwinMod = Modifier(modifiers);

		var kwinKey = Key(key);

		var kwinSequence = $"{kwinMod}+{kwinKey}";

		return kwinSequence;
	}

	private static string Key(Keys key) =>
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

			Add => "+",
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
			OemPipe => "|",
			OemQuestion => "?",
			OemQuotes => "\"",
			OemSemicolon => ";",
			Oemcomma => ",",
			Oemplus => "+",

			VolumeMute => "Volume Mute",
			VolumeDown => "Volume Down",
			VolumeUp => "Volume Up",

			_ => throw new WtqException($"Unsupported key '{key}'."),
		};

	private static string Modifier(KeyModifiers modifiers) =>
		modifiers switch
		{
			KeyModifiers.Control
				=> "Ctrl",
			KeyModifiers.Alt
				=> "Alt",
			KeyModifiers.Shift
				=> "Shift",
			KeyModifiers.Super
				=> "Meta",
			KeyModifiers.None
				=> throw new WtqException($"Unsupported modifier '{modifiers}'."),
			KeyModifiers.NoRepeat
				=> throw new WtqException($"Unsupported modifier '{modifiers}'."),
			_
				=> throw new WtqException($"Unsupported modifier '{modifiers}'."),
		};
}