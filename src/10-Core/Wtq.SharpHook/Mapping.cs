using SharpHook.Native;
using Wtq.Core.Data;

namespace Wtq.SharpHook;

public static class Mapping
{
	public static WtqKeys ToWtqKeys(this KeyCode keyCode)
	{
		switch (keyCode)
		{
			case KeyCode.Vc0: return WtqKeys.D0;
			case KeyCode.Vc1: return WtqKeys.D1;
			case KeyCode.Vc2: return WtqKeys.D2;
			case KeyCode.Vc3: return WtqKeys.D3;
			case KeyCode.Vc4: return WtqKeys.D4;
			case KeyCode.Vc5: return WtqKeys.D5;
			case KeyCode.Vc6: return WtqKeys.D6;
			case KeyCode.Vc7: return WtqKeys.D7;
			case KeyCode.Vc8: return WtqKeys.D8;
			case KeyCode.Vc9: return WtqKeys.D9;
			case KeyCode.VcBackQuote: return WtqKeys.Oemtilde;

			case KeyCode.VcQ: return WtqKeys.Q;

			default:
				return WtqKeys.None;
		}
	}

	public static WtqKeyModifiers ToWtqKeyModifiers(this ModifierMask mask)
	{
		switch (mask)
		{
			// Alt.
			case ModifierMask.Alt:
			case ModifierMask.LeftAlt:
			case ModifierMask.RightAlt:
				return WtqKeyModifiers.Alt;

			// Control.
			case ModifierMask.Ctrl:
			case ModifierMask.LeftCtrl:
			case ModifierMask.RightCtrl:
				return WtqKeyModifiers.Control;

			// Shift.
			case ModifierMask.Shift:
			case ModifierMask.LeftShift:
			case ModifierMask.RightShift:
				return WtqKeyModifiers.Shift;

			// Super.
			case ModifierMask.Meta:
			case ModifierMask.LeftMeta:
			case ModifierMask.RightMeta:
				return WtqKeyModifiers.Super;

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
				return WtqKeyModifiers.None;
		}
	}
}