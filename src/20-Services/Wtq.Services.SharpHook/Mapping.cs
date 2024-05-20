using SharpHook.Native;
using Wtq.Configuration;

namespace Wtq.Services.SharpHook;

public static class Mapping
{
	public static Keys ToWtqKeys(this KeyCode keyCode)
	{
		switch (keyCode)
		{
			// Esc, F-keys, etc.
			case KeyCode.VcEscape: return Keys.Escape;
			case KeyCode.VcF1: return Keys.F1;
			case KeyCode.VcF2: return Keys.F2;
			case KeyCode.VcF3: return Keys.F3;
			case KeyCode.VcF4: return Keys.F4;
			case KeyCode.VcF5: return Keys.F5;
			case KeyCode.VcF6: return Keys.F6;
			case KeyCode.VcF7: return Keys.F7;
			case KeyCode.VcF8: return Keys.F8;
			case KeyCode.VcF9: return Keys.F9;
			case KeyCode.VcF10: return Keys.F10;
			case KeyCode.VcF11: return Keys.F11;
			case KeyCode.VcF12: return Keys.F12;
			case KeyCode.VcF13: return Keys.F13;
			case KeyCode.VcF14: return Keys.F14;
			case KeyCode.VcF15: return Keys.F15;
			case KeyCode.VcF16: return Keys.F16;
			case KeyCode.VcF17: return Keys.F17;
			case KeyCode.VcF18: return Keys.F18;
			case KeyCode.VcF19: return Keys.F19;
			case KeyCode.VcF20: return Keys.F20;
			case KeyCode.VcF21: return Keys.F21;
			case KeyCode.VcF22: return Keys.F22;
			case KeyCode.VcF23: return Keys.F23;
			case KeyCode.VcF24: return Keys.F24;
			case KeyCode.VcPrintScreen: return Keys.PrintScreen;
			case KeyCode.VcScrollLock: return Keys.Scroll;
			case KeyCode.VcPause: return Keys.Pause;

			// ~, 0-9, etc.
			case KeyCode.VcBackQuote: return Keys.Oemtilde;
			case KeyCode.Vc0: return Keys.D0;
			case KeyCode.Vc1: return Keys.D1;
			case KeyCode.Vc2: return Keys.D2;
			case KeyCode.Vc3: return Keys.D3;
			case KeyCode.Vc4: return Keys.D4;
			case KeyCode.Vc5: return Keys.D5;
			case KeyCode.Vc6: return Keys.D6;
			case KeyCode.Vc7: return Keys.D7;
			case KeyCode.Vc8: return Keys.D8;
			case KeyCode.Vc9: return Keys.D9;
			case KeyCode.VcMinus: return Keys.OemMinus;
			case KeyCode.VcEquals: return Keys.Oemplus;
			case KeyCode.VcBackspace: return Keys.Back;

			case KeyCode.VcTab: return Keys.Tab;
			case KeyCode.VcCapsLock: return Keys.CapsLock;



			case KeyCode.VcA: return Keys.A;
			case KeyCode.VcB: return Keys.B;
			case KeyCode.VcC: return Keys.C;
			case KeyCode.VcD: return Keys.D;
			case KeyCode.VcE: return Keys.E;
			case KeyCode.VcF: return Keys.F;
			case KeyCode.VcG: return Keys.G;
			case KeyCode.VcH: return Keys.H;
			case KeyCode.VcI: return Keys.I;
			case KeyCode.VcJ: return Keys.J;
			case KeyCode.VcK: return Keys.K;
			case KeyCode.VcL: return Keys.L;
			case KeyCode.VcM: return Keys.M;
			case KeyCode.VcN: return Keys.N;
			case KeyCode.VcO: return Keys.O;
			case KeyCode.VcP: return Keys.P;
			case KeyCode.VcQ: return Keys.Q;
			case KeyCode.VcR: return Keys.R;
			case KeyCode.VcS: return Keys.S;
			case KeyCode.VcT: return Keys.T;
			case KeyCode.VcU: return Keys.U;
			case KeyCode.VcV: return Keys.V;
			case KeyCode.VcW: return Keys.W;
			case KeyCode.VcX: return Keys.X;
			case KeyCode.VcY: return Keys.Y;
			case KeyCode.VcZ: return Keys.Z;

			case KeyCode.VcOpenBracket: return Keys.OemOpenBrackets;
			case KeyCode.VcCloseBracket: return Keys.OemCloseBrackets;
			case KeyCode.VcBackslash: return Keys.OemBackslash;
			case KeyCode.VcSemicolon: return Keys.OemSemicolon;
			case KeyCode.VcQuote: return Keys.OemQuotes;
			case KeyCode.VcEnter: return Keys.Enter;
			case KeyCode.VcComma: return Keys.Oemcomma;
			case KeyCode.VcPeriod: return Keys.OemPeriod;
			case KeyCode.VcSlash: return Keys.OemBackslash;
			case KeyCode.VcSpace: return Keys.Space;



			case KeyCode.VcInsert: return Keys.Insert;
			case KeyCode.VcDelete: return Keys.Delete;
			case KeyCode.VcHome: return Keys.Home;
			case KeyCode.VcEnd: return Keys.End;
			case KeyCode.VcPageUp: return Keys.PageUp;
			case KeyCode.VcPageDown: return Keys.PageDown;

			case KeyCode.VcUp: return Keys.Up;
			case KeyCode.VcLeft: return Keys.Left;
			case KeyCode.VcRight: return Keys.Right;
			case KeyCode.VcDown: return Keys.Down;

			// Numpad.
			case KeyCode.VcNumLock: return Keys.NumLock;
			case KeyCode.VcNumPadDivide: return Keys.Divide;
			case KeyCode.VcNumPadMultiply: return Keys.Multiply;
			case KeyCode.VcNumPadSubtract: return Keys.Subtract;
			case KeyCode.VcNumPadAdd: return Keys.Add;
			case KeyCode.VcNumPadEnter: return Keys.Enter;
			case KeyCode.VcNumPadDecimal: return Keys.Decimal;
			case KeyCode.VcNumPadSeparator: return Keys.Separator;
			case KeyCode.VcNumPad0: return Keys.NumPad0;
			case KeyCode.VcNumPad1: return Keys.NumPad1;
			case KeyCode.VcNumPad2: return Keys.NumPad2;
			case KeyCode.VcNumPad3: return Keys.NumPad3;
			case KeyCode.VcNumPad4: return Keys.NumPad4;
			case KeyCode.VcNumPad5: return Keys.NumPad5;
			case KeyCode.VcNumPad6: return Keys.NumPad6;
			case KeyCode.VcNumPad7: return Keys.NumPad7;
			case KeyCode.VcNumPad8: return Keys.NumPad8;
			case KeyCode.VcNumPad9: return Keys.NumPad9;

			case KeyCode.VcLeftShift: return Keys.LShiftKey;
			case KeyCode.VcRightShift: return Keys.RShiftKey;
			case KeyCode.VcLeftControl: return Keys.LControlKey;
			case KeyCode.VcRightControl: return Keys.RControlKey;
			case KeyCode.VcLeftAlt: return Keys.Alt;
			case KeyCode.VcRightAlt: return Keys.Alt;
			case KeyCode.VcLeftMeta: return Keys.LWin;
			case KeyCode.VcRightMeta: return Keys.RWin;

			default:
				return Keys.None;
		}
	}

	public static KeyModifiers ToWtqKeyModifiers(this ModifierMask mask)
	{
		switch (mask)
		{
			// Alt.
			case ModifierMask.Alt:
			case ModifierMask.LeftAlt:
			case ModifierMask.RightAlt:
				return KeyModifiers.Alt;

			// Control.
			case ModifierMask.Ctrl:
			case ModifierMask.LeftCtrl:
			case ModifierMask.RightCtrl:
				return KeyModifiers.Control;

			// Shift.
			case ModifierMask.Shift:
			case ModifierMask.LeftShift:
			case ModifierMask.RightShift:
				return KeyModifiers.Shift;

			// Super.
			case ModifierMask.Meta:
			case ModifierMask.LeftMeta:
			case ModifierMask.RightMeta:
				return KeyModifiers.Super;

			case ModifierMask.Button1:
			case ModifierMask.Button2:
			case ModifierMask.Button3:
			case ModifierMask.Button4:
			case ModifierMask.Button5:
			case ModifierMask.NumLock:
			case ModifierMask.CapsLock:
			case ModifierMask.ScrollLock:
			case ModifierMask.None:
			default:
				return KeyModifiers.None;
		}
	}
}