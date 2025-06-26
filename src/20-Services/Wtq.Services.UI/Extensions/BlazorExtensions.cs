using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Wtq.Services.UI.Input;

namespace Wtq.Services.UI.Extensions;

public static class BlazorExtensions
{
	private static readonly ILogger _log = Log.For(typeof(BlazorExtensions));

	/// <summary>
	/// Converts the numeric value of the "location" property to an <see cref="Html5DomKeyLocation"/>.
	/// </summary>
	/// <remarks>https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent#keyboard_locations.</remarks>
	public static Html5DomKeyLocation GetKeyLocation(this KeyboardEventArgs e)
	{
		switch ((int)e.Location)
		{
			case 0: return Html5DomKeyLocation.Standard;
			case 1: return Html5DomKeyLocation.Left;
			case 2: return Html5DomKeyLocation.Right;
			case 3: return Html5DomKeyLocation.Numpad;

			default: return Html5DomKeyLocation.Unknown;
		}
	}

	public static void ToModifiersAndKey(
		this KeyboardEventArgs ev,
		out KeyModifiers mod,
		out string? keyChar,
		out KeyCode keyCode)
	{
		Guard.Against.Null(ev);

		// Modifier
		mod = ToKeyModifiers(ev);

		// Key char
		keyChar = ev.Key;

		// Key code
		keyCode = ToKeyCode(ev.Code);

		var loc = ev.GetKeyLocation();

		// If this key comes from the numpad, add the "Numpad" modifier.
		if (loc == Html5DomKeyLocation.Numpad)
		{
			mod |= KeyModifiers.Numpad;
		}
	}

	private static KeyModifiers ToKeyModifiers(KeyboardEventArgs ev)
	{
		var mod = KeyModifiers.None;

		if (ev.AltKey)
		{
			mod |= KeyModifiers.Alt;
		}

		if (ev.CtrlKey)
		{
			mod |= KeyModifiers.Control;
		}

		if (ev.ShiftKey)
		{
			mod |= KeyModifiers.Shift;
		}

		// Note that the "MetaKey" property doesn't seem to work on Photino+Linux.
		// This is handled through the KeyUp/KeyDown events.
		if (ev.MetaKey)
		{
			mod |= KeyModifiers.Super;
		}

		return mod;
	}

	/// <summary>
	/// <a href="https://www.toptal.com/developers/keycode/table"/>.
	/// </summary>
	private static KeyCode ToKeyCode(string code)
	{
		// @formatter:off
		#pragma warning disable SA1025
		#pragma warning disable SA1027

		switch (code.ToLowerInvariant())
		{
			case "pause":				return KeyCode.Pause;
			case "backquote":			return KeyCode.OemTilde;
			case "backspace":			return KeyCode.Backspace;
			case "tab":					return KeyCode.Tab;
			case "numlock":				return KeyCode.NumLock;
			case "enter":				return KeyCode.Return;
			case "capslock":			return KeyCode.CapsLock;
			case "escape":				return KeyCode.Return;
			case "minus":				return KeyCode.OemMinus;
			case "equal":				return KeyCode.OemPlus;
			case "insert":				return KeyCode.Insert;
			case "delete":				return KeyCode.Delete;
			case "bracketleft":			return KeyCode.OemOpenBrackets;
			case "bracketright":		return KeyCode.OemCloseBrackets;
			case "semicolon":			return KeyCode.OemSemicolon;
			case "quote":				return KeyCode.OemQuotes;
			case "period":				return KeyCode.OemPeriod;
			case "comma":				return KeyCode.OemComma;
			case "slash":				return KeyCode.OemQuestion;
			case "pageup":				return KeyCode.PageUp;
			case "pagedown":			return KeyCode.PageDown;
			case "arrowup":				return KeyCode.ArrowUp;
			case "arrowdown":			return KeyCode.ArrowDown;
			case "arrowleft":			return KeyCode.ArrowLeft;
			case "arrowright":			return KeyCode.ArrowRight;
			case "backslash":			return KeyCode.OemBackslash;
			case "space":				return KeyCode.Space;

			case "digit0":				return KeyCode.D0;
			case "digit1":				return KeyCode.D1;
			case "digit2":				return KeyCode.D2;
			case "digit3":				return KeyCode.D3;
			case "digit4":				return KeyCode.D4;
			case "digit5":				return KeyCode.D5;
			case "digit6":				return KeyCode.D6;
			case "digit7":				return KeyCode.D7;
			case "digit8":				return KeyCode.D8;
			case "digit9":				return KeyCode.D9;

			case "f1":					return KeyCode.F1;
			case "f2":					return KeyCode.F2;
			case "f3":					return KeyCode.F3;
			case "f4":					return KeyCode.F4;
			case "f5":					return KeyCode.F5;
			case "f6":					return KeyCode.F6;
			case "f7":					return KeyCode.F7;
			case "f8":					return KeyCode.F8;
			case "f9":					return KeyCode.F9;
			case "f10":					return KeyCode.F10;
			case "f11":					return KeyCode.F11;
			case "f12":					return KeyCode.F12;
			case "f13":					return KeyCode.F13;
			case "f14":					return KeyCode.F14;
			case "f15":					return KeyCode.F15;
			case "f16":					return KeyCode.F16;
			case "f17":					return KeyCode.F17;
			case "f18":					return KeyCode.F18;
			case "f19":					return KeyCode.F19;
			case "f20":					return KeyCode.F20;
			case "f21":					return KeyCode.F21;
			case "f22":					return KeyCode.F22;
			case "f23":					return KeyCode.F23;
			case "f24":					return KeyCode.F24;

			case "keya":				return KeyCode.A;
			case "keyb":				return KeyCode.B;
			case "keyc":				return KeyCode.C;
			case "keyd":				return KeyCode.D;
			case "keye":				return KeyCode.E;
			case "keyf":				return KeyCode.F;
			case "keyg":				return KeyCode.G;
			case "keyh":				return KeyCode.H;
			case "keyi":				return KeyCode.I;
			case "keyj":				return KeyCode.J;
			case "keyk":				return KeyCode.K;
			case "keyl":				return KeyCode.L;
			case "keym":				return KeyCode.M;
			case "keyn":				return KeyCode.N;
			case "keyo":				return KeyCode.O;
			case "keyp":				return KeyCode.P;
			case "keyq":				return KeyCode.Q;
			case "keyr":				return KeyCode.R;
			case "keys":				return KeyCode.S;
			case "keyt":				return KeyCode.T;
			case "keyu":				return KeyCode.U;
			case "keyv":				return KeyCode.V;
			case "keyw":				return KeyCode.W;
			case "keyx":				return KeyCode.X;
			case "keyy":				return KeyCode.Y;
			case "keyz":				return KeyCode.Z;

			case "numpad0":				return KeyCode.Numpad0;
			case "numpad1":				return KeyCode.Numpad1;
			case "numpad2":				return KeyCode.Numpad2;
			case "numpad3":				return KeyCode.Numpad3;
			case "numpad4":				return KeyCode.Numpad4;
			case "numpad5":				return KeyCode.Numpad5;
			case "numpad6":				return KeyCode.Numpad6;
			case "numpad7":				return KeyCode.Numpad7;
			case "numpad8":				return KeyCode.Numpad8;
			case "numpad9":				return KeyCode.Numpad9;
			case "numpaddivide":		return KeyCode.NumpadDivide;
			case "numpadmultiply":		return KeyCode.NumpadMultiply;
			case "numpadsubtract":		return KeyCode.NumpadSubtract;
			case "numpadadd":			return KeyCode.NumpadAdd;
			case "numpaddecimal":		return KeyCode.NumpadDecimal;
			case "numpadenter":			return KeyCode.Return;

			// Modifiers
			case "altleft":				return KeyCode.AltLeft;
			case "altright":			return KeyCode.AltRight;
			case "controlleft":			return KeyCode.ControlLeft;
			case "controlright":		return KeyCode.ControlRight;
			case "shiftleft":			return KeyCode.ShiftLeft;
			case "shiftright":			return KeyCode.ShiftRight;
			case "superleft":			return KeyCode.SuperLeft;
			case "superright":			return KeyCode.SuperRight;
			case "osleft":				return KeyCode.SuperLeft;
			case "osright":				return KeyCode.SuperRight;
		}

		#pragma warning restore SA1027
		#pragma warning restore SA1025

		// @formatter:off
		_log.LogWarning("Unknown key code '{Code}'", code);

		return KeyCode.None;
	}
}