using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Wtq.Configuration;
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

	/// <summary>
	/// Returns whether the "shift"-modifier has caused the pressed key to emit a symbol, unrelated to the one that
	/// would have been emitted, had shift not been pressed.<br/>
	/// <br/>
	/// For example: when pressing the "A" key without shift returns "a", with shift "A". These are different, but related.<br/>
	/// Pressing the "1" key on the main row of a US ANSI keyboard without shift returns "1", with shift "!". These are different, and not related.<br/>
	/// <br/>
	/// This is not a perfect method, but we need it for sending hotkey registrations to KWin, as there "shift" is not considered
	/// when the character already implies one.<br/>
	/// <br/>
	/// Nicer methods would probably require more access to the active keyboard layout and character mapping, which we don't have.
	/// </summary>
	public static bool HasShiftKeyAffectedCharacter(KeyModifiers modifiers, Keys? keyCode, string? keyChar)
	{
		// The "shift" key must be part of the active modifiers.
		if (!modifiers.HasShift())
		{
			return false;
		}

		// We can skip non-character keys.
		if (keyChar == null)
		{
			return false;
		}

		// Don't consider key chars that are referring to a non-character key, like "Tab", or "F1".
		if (keyChar.Length > 1)
		{
			return false;
		}

		return keyChar.All(c => !char.IsUpper(c) && !char.IsLower(c));
	}

	public static void ToModifiersAndKey(
		this KeyboardEventArgs ev,
		out KeyModifiers mod,
		out string? keyChar,
		out Keys keyCode)
	{
		Guard.Against.Null(ev);

		// Modifier
		mod = ToKeyModifiers(ev);

		// Key char
		keyChar = ev.Key;

		// Key code
		keyCode = ToKeys(ev.Code);

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
		if (ev.MetaKey)
		{
			mod |= KeyModifiers.Super;
		}

		return mod;
	}

	/// <summary>
	/// <a href="https://www.toptal.com/developers/keycode/table"/>.
	/// </summary>
	private static Keys ToKeys(string code)
	{
		switch (code.ToLowerInvariant())
		{
			case "pause": return Keys.Pause;
			case "backquote": return Keys.Oemtilde;
			case "backspace": return Keys.Back;
			case "tab": return Keys.Tab;
			case "numlock": return Keys.NumLock;
			case "enter": return Keys.Enter;
			case "capslock": return Keys.CapsLock;
			case "escape": return Keys.Escape;
			case "minus": return Keys.OemMinus;
			case "equal": return Keys.Oemplus;
			case "insert": return Keys.Insert;
			case "delete": return Keys.Delete;
			case "bracketleft": return Keys.OemOpenBrackets;
			case "bracketright": return Keys.OemCloseBrackets;
			case "semicolon": return Keys.OemSemicolon;
			case "quote": return Keys.OemQuotes;
			case "period": return Keys.OemPeriod;
			case "comma": return Keys.Oemcomma;
			case "slash": return Keys.OemQuestion;
			case "pageup": return Keys.PageUp;
			case "pagedown": return Keys.PageDown;
			case "arrowup": return Keys.Up;
			case "arrowdown": return Keys.Down;
			case "arrowleft": return Keys.Left;
			case "arrowright": return Keys.Right;
			case "backslash": return Keys.OemBackslash;
			case "space": return Keys.Space;

			case "digit0": return Keys.D0;
			case "digit1": return Keys.D1;
			case "digit2": return Keys.D2;
			case "digit3": return Keys.D3;
			case "digit4": return Keys.D4;
			case "digit5": return Keys.D5;
			case "digit6": return Keys.D6;
			case "digit7": return Keys.D7;
			case "digit8": return Keys.D8;
			case "digit9": return Keys.D9;

			case "f1": return Keys.F1;
			case "f2": return Keys.F2;
			case "f3": return Keys.F3;
			case "f4": return Keys.F4;
			case "f5": return Keys.F5;
			case "f6": return Keys.F6;
			case "f7": return Keys.F7;
			case "f8": return Keys.F8;
			case "f9": return Keys.F9;
			case "f10": return Keys.F10;
			case "f11": return Keys.F11;
			case "f12": return Keys.F12;
			case "f13": return Keys.F13;
			case "f14": return Keys.F14;
			case "f15": return Keys.F15;
			case "f16": return Keys.F16;
			case "f17": return Keys.F17;
			case "f18": return Keys.F18;
			case "f19": return Keys.F19;
			case "f20": return Keys.F20;
			case "f21": return Keys.F21;
			case "f22": return Keys.F22;
			case "f23": return Keys.F23;
			case "f24": return Keys.F24;

			case "keya": return Keys.A;
			case "keyb": return Keys.B;
			case "keyc": return Keys.C;
			case "keyd": return Keys.D;
			case "keye": return Keys.E;
			case "keyf": return Keys.F;
			case "keyg": return Keys.G;
			case "keyh": return Keys.H;
			case "keyi": return Keys.I;
			case "keyj": return Keys.J;
			case "keyk": return Keys.K;
			case "keyl": return Keys.L;
			case "keym": return Keys.M;
			case "keyn": return Keys.N;
			case "keyo": return Keys.O;
			case "keyp": return Keys.P;
			case "keyq": return Keys.Q;
			case "keyr": return Keys.R;
			case "keys": return Keys.S;
			case "keyt": return Keys.T;
			case "keyu": return Keys.U;
			case "keyv": return Keys.V;
			case "keyw": return Keys.W;
			case "keyx": return Keys.X;
			case "keyy": return Keys.Y;
			case "keyz": return Keys.Z;

			case "numpad0": return Keys.NumPad0;
			case "numpad1": return Keys.NumPad1;
			case "numpad2": return Keys.NumPad2;
			case "numpad3": return Keys.NumPad3;
			case "numpad4": return Keys.NumPad4;
			case "numpad5": return Keys.NumPad5;
			case "numpad6": return Keys.NumPad6;
			case "numpad7": return Keys.NumPad7;
			case "numpad8": return Keys.NumPad8;
			case "numpad9": return Keys.NumPad9;
			case "numpaddivide": return Keys.Divide;
			case "numpadmultiply": return Keys.Multiply;
			case "numpadsubtract": return Keys.Subtract;
			case "numpadadd": return Keys.Add;
			case "numpaddecimal": return Keys.Decimal;
			case "numpadenter": return Keys.Enter;

			// Modifiers
			// case "alt": return Keys.Alt;
			case "altleft": return Keys.LAltKey;
			case "altright": return Keys.RAltKey;

			// case "control": return Keys.Control;
			case "controlleft": return Keys.LControlKey;
			case "controlright": return Keys.RControlKey;

			// case "shift": return Keys.Shift;
			case "shiftleft": return Keys.LShiftKey;
			case "shiftright": return Keys.RShiftKey;

			// case "super": return Keys.Super;
			case "superleft": return Keys.LSuperKey;
			case "superright": return Keys.RSuperKey;
			case "osleft": return Keys.LSuperKey;
			case "osright": return Keys.RSuperKey;
		}

		_log.LogWarning("Unknown key code '{Code}'", code);

		return Keys.None;
	}
}