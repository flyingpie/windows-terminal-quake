using System.Text;

namespace Wtq.Services.KWin.Input;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "MvdO: Naming convention.")]
public static class KeySequenceExtensions
{
	public static string ToKWinString(this KeySequence sequence)
	{
		var sb = new StringBuilder();

		// TODO: Determine whether the "Shift" key is required.
		// I.e., with sequence "Shift+F1", it _is_ required, as the actual key isn't affected by the shift.
		// But with "Ctrl+Shift+1", it _is not_ required, as it will be bound as "Ctrl+@" (on US keyboards).

		// Modifier
		if (sequence.Modifiers != KeyModifiers.None)
		{
			sb.Append(sequence.Modifiers.ToKWinString());
		}

		// Key char
		// KWin uses key characters, so prefer that one if we have it right in the settings.
		if (sequence.HasKeyChar)
		{
			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			sb.Append(sequence.KeyChar.ToKWinString());
		}

		// Alternatively, if no key character is present, map the key code.
		// This is not a perfect method, as the character depends on the keyboard layout, which I haven't found a way to access yet.
		else if (sequence.HasKeyCode)
		{
			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			var keyCodeStr = sequence.KeyCode.ToKWinString();
			if (!string.IsNullOrWhiteSpace(keyCodeStr))
			{
				sb.Append(keyCodeStr);
			}
		}

		return sb.ToString();
	}

	/// <summary>
	/// Converts 0 or more modifiers into a string that can be used for registration in KWin.
	/// </summary>
	private static string ToKWinString(this KeyModifiers modifiers)
	{
		switch (modifiers)
		{
			case KeyModifiers.Control:
				return "Ctrl";
			case KeyModifiers.Alt:
				return "Alt";
			case KeyModifiers.Shift:
				return "Shift";
			case KeyModifiers.Super:
				return "Meta";
			case KeyModifiers.Numpad:
				return "Num";

			case KeyModifiers.None:
			default:
				return string.Empty;
		}
	}

	/// <summary>
	/// Converts no- or 1 key into a string that can be used for registration in KWin.
	/// </summary>
	private static string? ToKWinString(this KeyCode keyCode)
	{
		// @formatter:off
		#pragma warning disable SA1025
		#pragma warning disable SA1027

		switch (keyCode)
		{
			// The Most Popular Keys
			case KeyCode.Backspace:					return "Backspace";
			case KeyCode.Escape:					return "Esc";
			case KeyCode.Pause:						return "Pause";
			case KeyCode.PrintScreen:				return "Print";
			case KeyCode.Return:					return "Esc";
			case KeyCode.Space:						return "Space";
			case KeyCode.Tab:						return "Tab";
			case KeyCode.ContextMenu:				return null; // TODO: Unmapped

			// IME
			case KeyCode.FinalMode:					return null; // TODO: Unmapped
			case KeyCode.HanjaMode:					return null; // TODO: Unmapped
			case KeyCode.IMEAccept:					return null; // TODO: Unmapped
			case KeyCode.IMEConvert:				return null; // TODO: Unmapped
			case KeyCode.IMEModeChange:				return null; // TODO: Unmapped
			case KeyCode.IMENonConvert:				return null; // TODO: Unmapped
			case KeyCode.JunjaMode:					return null; // TODO: Unmapped
			case KeyCode.KanaMode:					return null; // TODO: Unmapped

			// Arrow keys
			case KeyCode.ArrowDown:					return "Down";
			case KeyCode.ArrowLeft:					return "Left";
			case KeyCode.ArrowRight:				return "Right";
			case KeyCode.ArrowUp:					return "Up";

			// Above arrow keys
			case KeyCode.Delete:					return "Delete";
			case KeyCode.End:						return "End";
			case KeyCode.Home:						return "Home";
			case KeyCode.Insert:					return "Insert";
			case KeyCode.PageDown:					return "PgDown";
			case KeyCode.PageUp:					return "PgUp";

			// Modifiers
			case KeyCode.AltLeft:					return "Alt";
			case KeyCode.AltRight:					return "Alt";
			case KeyCode.ControlLeft:				return "Control";
			case KeyCode.ControlRight:				return "Control";
			case KeyCode.ShiftLeft:					return "Shift";
			case KeyCode.ShiftRight:				return "Shift";
			case KeyCode.SuperLeft:					return "Meta";
			case KeyCode.SuperRight:				return "Meta";

			// Main row numbers
			case KeyCode.D0:						return "0";
			case KeyCode.D1:						return "1";
			case KeyCode.D2:						return "2";
			case KeyCode.D3:						return "3";
			case KeyCode.D4:						return "4";
			case KeyCode.D5:						return "5";
			case KeyCode.D6:						return "6";
			case KeyCode.D7:						return "7";
			case KeyCode.D8:						return "8";
			case KeyCode.D9:						return "9";

			// Main row
			case KeyCode.A:							return "A";
			case KeyCode.B:							return "B";
			case KeyCode.C:							return "C";
			case KeyCode.D:							return "D";
			case KeyCode.E:							return "E";
			case KeyCode.F:							return "F";
			case KeyCode.G:							return "G";
			case KeyCode.H:							return "H";
			case KeyCode.I:							return "I";
			case KeyCode.J:							return "J";
			case KeyCode.K:							return "K";
			case KeyCode.L:							return "L";
			case KeyCode.M:							return "M";
			case KeyCode.N:							return "N";
			case KeyCode.O:							return "O";
			case KeyCode.P:							return "P";
			case KeyCode.Q:							return "Q";
			case KeyCode.R:							return "R";
			case KeyCode.S:							return "S";
			case KeyCode.T:							return "T";
			case KeyCode.U:							return "U";
			case KeyCode.V:							return "V";
			case KeyCode.W:							return "W";
			case KeyCode.X:							return "X";
			case KeyCode.Y:							return "Y";
			case KeyCode.Z:							return "Z";

			// OEM Keys
			case KeyCode.OemSemicolon:				return ";";
			case KeyCode.OemPlus:					return "=";
			case KeyCode.OemComma:					return ",";
			case KeyCode.OemMinus:					return "-";
			case KeyCode.OemPeriod:					return ".";
			case KeyCode.OemQuestion:				return "/";
			case KeyCode.OemTilde:					return "`";
			case KeyCode.OemPipe:					return "\\";
			case KeyCode.OemQuotes:					return "'";
			case KeyCode.OemOpenBrackets:			return "[";
			case KeyCode.OemCloseBrackets:			return "]";
			case KeyCode.Oem8:						return null; // TODO: Unmapped
			case KeyCode.Oem102:					return null; // TODO: Unmapped
			case KeyCode.OemClear:					return null; // TODO: Unmapped

			// Numpad
			case KeyCode.Numpad0:					return "Num+0";
			case KeyCode.Numpad1:					return "Num+1";
			case KeyCode.Numpad2:					return "Num+2";
			case KeyCode.Numpad3:					return "Num+3";
			case KeyCode.Numpad4:					return "Num+4";
			case KeyCode.Numpad5:					return "Num+5";
			case KeyCode.Numpad6:					return "Num+6";
			case KeyCode.Numpad7:					return "Num+7";
			case KeyCode.Numpad8:					return "Num+8";
			case KeyCode.Numpad9:					return "Num+9";
			case KeyCode.NumpadAdd:					return "Num++";
			case KeyCode.NumpadDecimal:				return "Num+.";
			case KeyCode.NumpadDivide:				return "Num+/";
			case KeyCode.NumpadMultiply:			return "Num+*";
			case KeyCode.NumpadSubtract:			return "Num+-";
			case KeyCode.NumpadSeparator:			return null; // TODO: Unmapped

			case KeyCode.F1:						return "F1";
			case KeyCode.F2:						return "F2";
			case KeyCode.F3:						return "F3";
			case KeyCode.F4:						return "F4";
			case KeyCode.F5:						return "F5";
			case KeyCode.F6:						return "F6";
			case KeyCode.F7:						return "F7";
			case KeyCode.F8:						return "F8";
			case KeyCode.F9:						return "F9";
			case KeyCode.F10:						return "F10";
			case KeyCode.F11:						return "F11";
			case KeyCode.F12:						return "F12";
			case KeyCode.F13:						return "F13";
			case KeyCode.F14:						return "F14";
			case KeyCode.F15:						return "F15";
			case KeyCode.F16:						return "F16";
			case KeyCode.F17:						return "F17";
			case KeyCode.F18:						return "F18";
			case KeyCode.F19:						return "F19";
			case KeyCode.F20:						return "F20";
			case KeyCode.F21:						return "F21";
			case KeyCode.F22:						return "F22";
			case KeyCode.F23:						return "F23";
			case KeyCode.F24:						return "F24";

			// Lock keys
			case KeyCode.CapsLock:					return "CapsLock";
			case KeyCode.NumLock:					return "NumLock";
			case KeyCode.ScrollLock:				return "ScrollLock";

			// Browser
			case KeyCode.BrowserBack:				return null; // TODO: Unmapped
			case KeyCode.BrowserForward:			return null; // TODO: Unmapped
			case KeyCode.BrowserRefresh:			return null; // TODO: Unmapped
			case KeyCode.BrowserStop:				return null; // TODO: Unmapped
			case KeyCode.BrowserSearch:				return null; // TODO: Unmapped
			case KeyCode.BrowserFavorites:			return null; // TODO: Unmapped
			case KeyCode.BrowserHome:				return null; // TODO: Unmapped

			// Volume
			case KeyCode.VolumeMute:				return "Volume Mute";	// TODO: Verify
			case KeyCode.VolumeDown:				return "Volume Down";	// TODO: Verify
			case KeyCode.VolumeUp:					return "Volume Up";		// TODO: Verify

			// Media
			case KeyCode.MediaNextTrack:			return null; // TODO: Unmapped
			case KeyCode.MediaPreviousTrack:		return null; // TODO: Unmapped
			case KeyCode.MediaStop:					return null; // TODO: Unmapped
			case KeyCode.MediaPlayPause:			return null; // TODO: Unmapped

			// Misc
			case KeyCode.LineFeed:					return null; // TODO: Unmapped
			case KeyCode.Clear:						return null; // TODO: Unmapped
			case KeyCode.Cancel:					return null; // TODO: Unmapped
			case KeyCode.Print:						return null; // TODO: Unmapped
			case KeyCode.Select:					return null; // TODO: Unmapped
			case KeyCode.Execute:					return null; // TODO: Unmapped
			case KeyCode.Help:						return null; // TODO: Unmapped
			case KeyCode.Sleep:						return null; // TODO: Unmapped
			case KeyCode.LaunchMail:				return null; // TODO: Unmapped
			case KeyCode.LaunchSelectMedia:			return null; // TODO: Unmapped
			case KeyCode.LaunchApplication1:		return null; // TODO: Unmapped
			case KeyCode.LaunchApplication2:		return null; // TODO: Unmapped
			case KeyCode.Process:					return null; // TODO: Unmapped
			case KeyCode.Packet:					return null; // TODO: Unmapped
			case KeyCode.Attn:						return null; // TODO: Unmapped
			case KeyCode.Crsel:						return null; // TODO: Unmapped
			case KeyCode.Exsel:						return null; // TODO: Unmapped
			case KeyCode.EraseEof:					return null; // TODO: Unmapped
			case KeyCode.Play:						return null; // TODO: Unmapped
			case KeyCode.Zoom:						return null; // TODO: Unmapped
			case KeyCode.NoName:					return null; // TODO: Unmapped
			case KeyCode.Pa1:						return null; // TODO: Unmapped

			case KeyCode.None:
			default:
				return null;
		}

		#pragma warning restore SA1027
		#pragma warning restore SA1025

		// @formatter:off
	}

	private static string ToKWinString(this string keyChar)
	{
		return Enum.TryParse<KeyCode>(keyChar, ignoreCase: true, out var code) ? code.ToKWinString() ?? keyChar : keyChar;
	}
}