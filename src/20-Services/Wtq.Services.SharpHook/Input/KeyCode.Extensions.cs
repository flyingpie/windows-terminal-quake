using SKC = SharpHook.Data.KeyCode;
using WKC = Wtq.Input.KeyCode;

namespace Wtq.Services.SharpHook.Input;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "MvdO: Naming convention.")]
public static class KeyCodeExtensions
{
	public static WKC ToWtqKeyCode(this SKC shKeyCode)
	{
		// @formatter:off
#pragma warning disable SA1025
#pragma warning disable SA1027
		switch (shKeyCode)
		{
			// Modifiers
			case SKC.VcLeftShift:			return WKC.LShift;
			case SKC.VcRightShift:			return WKC.RShift;
			case SKC.VcLeftControl:			return WKC.LControl;
			case SKC.VcRightControl:		return WKC.RControl;
			case SKC.VcLeftAlt:				return WKC.LAlt;
			case SKC.VcRightAlt:			return WKC.RAlt;
			case SKC.VcLeftMeta:			return WKC.LSuper;
			case SKC.VcRightMeta:			return WKC.RSuper;
			case SKC.VcContextMenu:			return WKC.None; // TODO: Unmapped
			case SKC.VcFunction:			return WKC.None; // TODO: Unmapped

			// Misc
			case SKC.VcBackQuote:			return WKC.Oemtilde;
			case SKC.VcEscape:				return WKC.Escape;
			case SKC.VcEnter:				return WKC.Enter;
			case SKC.VcSpace:				return WKC.Space;
			case SKC.VcTab:					return WKC.Tab;
			case SKC.VcPrintScreen:			return WKC.PrintScreen;
			case SKC.VcPause:				return WKC.Pause;
			case SKC.VcBackspace:			return WKC.Backspace;
			case SKC.VcCancel:				return WKC.Cancel;
			case SKC.VcHelp:				return WKC.Help;
			case SKC.VcSleep:				return WKC.Sleep;
			case SKC.Vc102:					return WKC.Oem102;
			case SKC.VcPower:				return WKC.None; // TODO: Unmapped
			case SKC.VcUnderscore:			return WKC.None; // TODO: Unmapped
			case SKC.VcYen:					return WKC.None; // TODO: Unmapped
			case SKC.VcJpComma:				return WKC.None; // TODO: Unmapped
			case SKC.VcSlash:				return WKC.None; // TODO: Unmapped
			case SKC.VcMisc:				return WKC.None; // TODO: Unmapped
			case SKC.VcEquals:				return WKC.None; // TODO: Unmapped
			case SKC.VcChangeInputSource:	return WKC.None; // TODO: Unmapped

			// Right of main row
			case SKC.VcOpenBracket:			return WKC.OemOpenBrackets;
			case SKC.VcCloseBracket:		return WKC.OemCloseBrackets;
			case SKC.VcBackslash:			return WKC.OemBackslash;
			case SKC.VcSemicolon:			return WKC.OemSemicolon;
			case SKC.VcQuote:				return WKC.OemQuotes;
			case SKC.VcComma:				return WKC.Oemcomma;
			case SKC.VcPeriod:				return WKC.OemPeriod;

			// Above arrow keys
			case SKC.VcInsert:				return WKC.Insert;
			case SKC.VcDelete:				return WKC.Delete;
			case SKC.VcHome:				return WKC.Home;
			case SKC.VcEnd:					return WKC.End;
			case SKC.VcPageUp:				return WKC.PageUp;
			case SKC.VcPageDown:			return WKC.PageDown;

			// Arrow keys
			case SKC.VcUp:					return WKC.ArrowUp;
			case SKC.VcLeft:				return WKC.ArrowLeft;
			case SKC.VcRight:				return WKC.ArrowRight;
			case SKC.VcDown:				return WKC.ArrowDown;
			case SKC.VcMinus:				return WKC.OemMinus;

			// Media keys
			case SKC.VcMediaPlay:			return WKC.MediaPlayPause;
			case SKC.VcMediaStop:			return WKC.MediaStop;
			case SKC.VcMediaPrevious:		return WKC.MediaPreviousTrack;
			case SKC.VcMediaNext:			return WKC.MediaNextTrack;
			case SKC.VcMediaSelect:			return WKC.SelectMedia;
			case SKC.VcVolumeMute:			return WKC.VolumeMute;
			case SKC.VcVolumeDown:			return WKC.VolumeDown;
			case SKC.VcVolumeUp:			return WKC.VolumeUp;
			case SKC.VcMediaEject:			return WKC.None; // TODO: Unmapped

			case SKC.VcApp1:				return WKC.LaunchApplication1;
			case SKC.VcApp2:				return WKC.LaunchApplication2;
			case SKC.VcAppMail:				return WKC.LaunchMail;
			case SKC.VcApp3:				return WKC.None; // TODO: Unmapped
			case SKC.VcApp4:				return WKC.None; // TODO: Unmapped
			case SKC.VcAppBrowser:			return WKC.None; // TODO: Unmapped
			case SKC.VcAppCalculator:		return WKC.None; // TODO: Unmapped

			case SKC.VcBrowserSearch:		return WKC.BrowserSearch;
			case SKC.VcBrowserHome:			return WKC.BrowserHome;
			case SKC.VcBrowserBack:			return WKC.BrowserBack;
			case SKC.VcBrowserForward:		return WKC.BrowserForward;
			case SKC.VcBrowserStop:			return WKC.BrowserStop;
			case SKC.VcBrowserRefresh:		return WKC.BrowserRefresh;
			case SKC.VcBrowserFavorites:	return WKC.BrowserFavorites;

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