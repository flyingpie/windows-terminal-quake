using SharpHook.Native;
using Wtq.Configuration;

namespace Wtq.Services.SharpHook;

public static class Mapping
{
	public static Keys ToWtqKeys(this KeyCode keyCode)
	{
		switch (keyCode)
		{
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
			case KeyCode.VcBackQuote: return Keys.Oemtilde;

			case KeyCode.VcQ: return Keys.Q;

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