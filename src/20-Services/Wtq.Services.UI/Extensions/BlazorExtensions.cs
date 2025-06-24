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
			case "pause": return KeyCodePause;
			case "backquote": return KeyCodeOemtilde;
			case "backspace": return KeyCodeBack;
			case "tab": return KeyCodeTab;
			case "numlock": return KeyCodeNumLock;
			case "enter": return KeyCodeEnter;
			case "capslock": return KeyCodeCapsLock;
			case "escape": return KeyCodeEscape;
			case "minus": return KeyCodeOemMinus;
			case "equal": return KeyCodeOemplus;
			case "insert": return KeyCodeInsert;
			case "delete": return KeyCodeDelete;
			case "bracketleft": return KeyCodeOemOpenBrackets;
			case "bracketright": return KeyCodeOemCloseBrackets;
			case "semicolon": return KeyCodeOemSemicolon;
			case "quote": return KeyCodeOemQuotes;
			case "period": return KeyCodeOemPeriod;
			case "comma": return KeyCodeOemcomma;
			case "slash": return KeyCodeOemQuestion;
			case "pageup": return KeyCodePageUp;
			case "pagedown": return KeyCodePageDown;
			case "arrowup": return KeyCodeUp;
			case "arrowdown": return KeyCodeDown;
			case "arrowleft": return KeyCodeLeft;
			case "arrowright": return KeyCodeRight;
			case "backslash": return KeyCodeOemBackslash;
			case "space": return KeyCodeSpace;

			case "digit0": return KeyCodeD0;
			case "digit1": return KeyCodeD1;
			case "digit2": return KeyCodeD2;
			case "digit3": return KeyCodeD3;
			case "digit4": return KeyCodeD4;
			case "digit5": return KeyCodeD5;
			case "digit6": return KeyCodeD6;
			case "digit7": return KeyCodeD7;
			case "digit8": return KeyCodeD8;
			case "digit9": return KeyCodeD9;

			case "f1": return KeyCodeF1;
			case "f2": return KeyCodeF2;
			case "f3": return KeyCodeF3;
			case "f4": return KeyCodeF4;
			case "f5": return KeyCodeF5;
			case "f6": return KeyCodeF6;
			case "f7": return KeyCodeF7;
			case "f8": return KeyCodeF8;
			case "f9": return KeyCodeF9;
			case "f10": return KeyCodeF10;
			case "f11": return KeyCodeF11;
			case "f12": return KeyCodeF12;
			case "f13": return KeyCodeF13;
			case "f14": return KeyCodeF14;
			case "f15": return KeyCodeF15;
			case "f16": return KeyCodeF16;
			case "f17": return KeyCodeF17;
			case "f18": return KeyCodeF18;
			case "f19": return KeyCodeF19;
			case "f20": return KeyCodeF20;
			case "f21": return KeyCodeF21;
			case "f22": return KeyCodeF22;
			case "f23": return KeyCodeF23;
			case "f24": return KeyCodeF24;

			case "keya": return KeyCodeA;
			case "keyb": return KeyCodeB;
			case "keyc": return KeyCodeC;
			case "keyd": return KeyCodeD;
			case "keye": return KeyCodeE;
			case "keyf": return KeyCodeF;
			case "keyg": return KeyCodeG;
			case "keyh": return KeyCodeH;
			case "keyi": return KeyCodeI;
			case "keyj": return KeyCodeJ;
			case "keyk": return KeyCodeK;
			case "keyl": return KeyCodeL;
			case "keym": return KeyCodeM;
			case "keyn": return KeyCodeN;
			case "keyo": return KeyCodeO;
			case "keyp": return KeyCodeP;
			case "keyq": return KeyCodeQ;
			case "keyr": return KeyCodeR;
			case "keys": return KeyCodeS;
			case "keyt": return KeyCodeT;
			case "keyu": return KeyCodeU;
			case "keyv": return KeyCodeV;
			case "keyw": return KeyCodeW;
			case "keyx": return KeyCodeX;
			case "keyy": return KeyCodeY;
			case "keyz": return KeyCodeZ;

			case "numpad0": return KeyCodeNumPad0;
			case "numpad1": return KeyCodeNumPad1;
			case "numpad2": return KeyCodeNumPad2;
			case "numpad3": return KeyCodeNumPad3;
			case "numpad4": return KeyCodeNumPad4;
			case "numpad5": return KeyCodeNumPad5;
			case "numpad6": return KeyCodeNumPad6;
			case "numpad7": return KeyCodeNumPad7;
			case "numpad8": return KeyCodeNumPad8;
			case "numpad9": return KeyCodeNumPad9;
			case "numpaddivide": return KeyCodeDivide;
			case "numpadmultiply": return KeyCodeMultiply;
			case "numpadsubtract": return KeyCodeSubtract;
			case "numpadadd": return KeyCodeAdd;
			case "numpaddecimal": return KeyCodeDecimal;
			case "numpadenter": return KeyCodeEnter;

			// Modifiers
			// case "alt": return KeyCodeAlt;
			case "altleft": return KeyCodeLAltKey;
			case "altright": return KeyCodeRAltKey;

			// case "control": return KeyCodeControl;
			case "controlleft": return KeyCodeLControlKey;
			case "controlright": return KeyCodeRControlKey;

			// case "shift": return KeyCodeShift;
			case "shiftleft": return KeyCodeLShiftKey;
			case "shiftright": return KeyCodeRShiftKey;

			// case "super": return KeyCodeSuper;
			case "superleft": return KeyCodeLSuperKey;
			case "superright": return KeyCodeRSuperKey;
			case "osleft": return KeyCodeLSuperKey;
			case "osright": return KeyCodeRSuperKey;
		}

		_log.LogWarning("Unknown key code '{Code}'", code);

		return KeyCodeNone;
	}
}