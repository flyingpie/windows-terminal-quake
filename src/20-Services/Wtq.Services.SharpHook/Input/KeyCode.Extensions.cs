using SKC = SharpHook.Data.KeyCode;
using WKC = Wtq.Input.KeyCode;

namespace Wtq.Services.SharpHook.Input;

public class ShWtqMapping
{
	public SKC SharpHookKeyCode { get; set; }

	public WKC WtqKeyCode { get; set; }
}

public static class KeyCodeExtensions
{
	public static WKC ToWtqKeyCode(SKC shKeyCode)
	{
		// @formatter:off
#pragma warning disable SA1025
#pragma warning disable SA1027
		switch (shKeyCode)
		{
			// Modifiers
			case SKC.VcLeftShift: return WKC.LShiftKey;
			case SKC.VcRightShift: return WKC.RShiftKey;
			case SKC.VcLeftControl: return WKC.LControlKey;
			case SKC.VcRightControl: return WKC.RControlKey;
			case SKC.VcLeftAlt: return WKC.LAltKey;
			case SKC.VcRightAlt: return WKC.RAltKey;
			case SKC.VcLeftMeta: return WKC.LSuperKey;
			case SKC.VcRightMeta: return WKC.RSuperKey;
			case SKC.VcContextMenu: return WKC.None; // TODO: Unmapped
			case SKC.VcFunction: return WKC.None; // TODO: Unmapped

			// Misc
			case SKC.VcBackQuote:
			case SKC.VcEscape:
			case SKC.VcOpenBracket:
			case SKC.VcCloseBracket:
			case SKC.VcBackslash:
			case SKC.VcSemicolon:
			case SKC.VcQuote:
			case SKC.VcEnter:
			case SKC.VcComma:
			case SKC.VcPeriod:
			case SKC.VcSlash:
			case SKC.VcSpace:
			case SKC.Vc102:
			case SKC.VcMisc:
			case SKC.VcPrintScreen:
			case SKC.VcPause:
			case SKC.VcCancel:
			case SKC.VcHelp:
			case SKC.VcInsert:
			case SKC.VcDelete:
			case SKC.VcHome:
			case SKC.VcEnd:
			case SKC.VcPageUp:
			case SKC.VcPageDown:
			case SKC.VcUp:
			case SKC.VcLeft:
			case SKC.VcRight:
			case SKC.VcDown:
			case SKC.VcMinus:
			case SKC.VcEquals:
			case SKC.VcBackspace:
			case SKC.VcTab:
			case SKC.VcChangeInputSource:
			case SKC.VcPower:
			case SKC.VcSleep:
			case SKC.VcMediaPlay:
			case SKC.VcMediaStop:
			case SKC.VcMediaPrevious:
			case SKC.VcMediaNext:
			case SKC.VcMediaSelect:
			case SKC.VcMediaEject:
			case SKC.VcVolumeMute:
			case SKC.VcVolumeDown:
			case SKC.VcVolumeUp:
			case SKC.VcApp1:
			case SKC.VcApp2:
			case SKC.VcApp3:
			case SKC.VcApp4:
			case SKC.VcAppBrowser:
			case SKC.VcAppCalculator:
			case SKC.VcAppMail:
			case SKC.VcBrowserSearch:
			case SKC.VcBrowserHome:
			case SKC.VcBrowserBack:
			case SKC.VcBrowserForward:
			case SKC.VcBrowserStop:
			case SKC.VcBrowserRefresh:
			case SKC.VcBrowserFavorites:
			case SKC.VcUnderscore:
			case SKC.VcYen:
			case SKC.VcJpComma:

			// IME
			case SKC.VcAccept:				return WKC.IMEAccept;
			case SKC.VcConvert:				return WKC.IMEConvert;
			case SKC.VcFinal:				return WKC.FinalMode;
			case SKC.VcHangul:				return WKC.HangulMode;
			case SKC.VcHanja:				return WKC.HanjaMode;
			case SKC.VcJunja:				return WKC.JunjaMode;
			case SKC.VcKana:				return WKC.KanaMode;
			case SKC.VcKanji:				return WKC.KanjiMode;
			case SKC.VcKatakanaHiragana:	return WKC.FinalMode;
			case SKC.VcModeChange:			return WKC.IMEModeChange;
			case SKC.VcNonConvert:			return WKC.IMENonconvert;
			case SKC.VcAlphanumeric:		return WKC.None; // TODO: Unmapped
			case SKC.VcHiragana:			return WKC.None; // TODO: Unmapped
			case SKC.VcImeOff:				return WKC.None; // TODO: Unmapped
			case SKC.VcImeOn:				return WKC.None; // TODO: Unmapped
			case SKC.VcKatakana:			return WKC.None; // TODO: Unmapped
			case SKC.VcProcess:				return WKC.None; // TODO: Unmapped

			// Lock keys
			case SKC.VcCapsLock:			return WKC.CapsLock;
			case SKC.VcNumLock:				return WKC.NumLock;
			case SKC.VcScrollLock:			return WKC.ScrollLock;

			// F1-F24
			case SKC.VcF1:					return WKC.F1;
			case SKC.VcF2:					return WKC.F2;
			case SKC.VcF3:					return WKC.F3;
			case SKC.VcF4:					return WKC.F4;
			case SKC.VcF5:					return WKC.F5;
			case SKC.VcF6:					return WKC.F6;
			case SKC.VcF7:					return WKC.F7;
			case SKC.VcF8:					return WKC.F8;
			case SKC.VcF9:					return WKC.F9;
			case SKC.VcF10:					return WKC.F10;
			case SKC.VcF11:					return WKC.F11;
			case SKC.VcF12:					return WKC.F12;
			case SKC.VcF13:					return WKC.F13;
			case SKC.VcF14:					return WKC.F14;
			case SKC.VcF15:					return WKC.F15;
			case SKC.VcF16:					return WKC.F16;
			case SKC.VcF17:					return WKC.F17;
			case SKC.VcF18:					return WKC.F18;
			case SKC.VcF19:					return WKC.F19;
			case SKC.VcF20:					return WKC.F20;
			case SKC.VcF21:					return WKC.F21;
			case SKC.VcF22:					return WKC.F22;
			case SKC.VcF23:					return WKC.F23;
			case SKC.VcF24:					return WKC.F24;

			// Digit 0-9
			case SKC.Vc0: 					return WKC.D0;
			case SKC.Vc1: 					return WKC.D1;
			case SKC.Vc2: 					return WKC.D2;
			case SKC.Vc3: 					return WKC.D3;
			case SKC.Vc4: 					return WKC.D4;
			case SKC.Vc5: 					return WKC.D5;
			case SKC.Vc6: 					return WKC.D6;
			case SKC.Vc7: 					return WKC.D7;
			case SKC.Vc8: 					return WKC.D8;
			case SKC.Vc9: 					return WKC.D9;

			// A-Z
			case SKC.VcA:					return WKC.A;
			case SKC.VcB:					return WKC.B;
			case SKC.VcC:					return WKC.C;
			case SKC.VcD:					return WKC.D;
			case SKC.VcE:					return WKC.E;
			case SKC.VcF:					return WKC.F;
			case SKC.VcG:					return WKC.G;
			case SKC.VcH:					return WKC.H;
			case SKC.VcI:					return WKC.I;
			case SKC.VcJ:					return WKC.J;
			case SKC.VcK:					return WKC.K;
			case SKC.VcL:					return WKC.L;
			case SKC.VcM:					return WKC.M;
			case SKC.VcN:					return WKC.N;
			case SKC.VcO:					return WKC.O;
			case SKC.VcP:					return WKC.P;
			case SKC.VcQ:					return WKC.Q;
			case SKC.VcR:					return WKC.R;
			case SKC.VcS:					return WKC.S;
			case SKC.VcT:					return WKC.T;
			case SKC.VcU:					return WKC.U;
			case SKC.VcV:					return WKC.V;
			case SKC.VcW:					return WKC.W;
			case SKC.VcX:					return WKC.X;
			case SKC.VcY:					return WKC.Y;
			case SKC.VcZ:					return WKC.Z;

			// Numpad
			case SKC.VcNumPadClear:			return WKC.Clear;
			case SKC.VcNumPadDivide:		return WKC.Divide;
			case SKC.VcNumPadMultiply:		return WKC.Multiply;
			case SKC.VcNumPadSubtract:		return WKC.Subtract;
			case SKC.VcNumPadAdd:			return WKC.Add;
			case SKC.VcNumPadDecimal:		return WKC.Decimal;
			case SKC.VcNumPadSeparator:		return WKC.Separator;
			case SKC.VcNumPad0:				return WKC.NumPad0;
			case SKC.VcNumPad1:				return WKC.NumPad1;
			case SKC.VcNumPad2:				return WKC.NumPad2;
			case SKC.VcNumPad3:				return WKC.NumPad3;
			case SKC.VcNumPad4:				return WKC.NumPad4;
			case SKC.VcNumPad5:				return WKC.NumPad5;
			case SKC.VcNumPad6:				return WKC.NumPad6;
			case SKC.VcNumPad7:				return WKC.NumPad7;
			case SKC.VcNumPad8:				return WKC.NumPad8;
			case SKC.VcNumPad9:				return WKC.NumPad9;
			case SKC.VcNumPadEnter:			return WKC.None; // TODO: Unmapped
			case SKC.VcNumPadEquals:		return WKC.None; // TODO: Unmapped

			case SKC.VcUndefined:
			default:
				return WKC.None;
		}

		// @formatter:on
#pragma warning restore SA1027
#pragma warning restore SA1025
	}
}