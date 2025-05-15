using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Wtq.Configuration;

namespace Wtq.Services.UI.Extensions;

public static class BlazorExtensions
{
	private static readonly ILogger _log = Log.For(typeof(BlazorExtensions));

	public static void ToModifiersAndKey(this KeyboardEventArgs ev, out KeyModifiers mod, out Keys key)
	{
		Guard.Against.Null(ev);

		mod = ToKeyModifiers(ev);
		key = ToKeys(ev.Code);
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

		// TODO: Super?

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
			case "numpaddivide": return Keys.Divide;
			case "numpadmultiply": return Keys.Multiply;
			case "numpadsubtract": return Keys.Subtract;
			case "numpadadd": return Keys.Add;
			case "numpaddecimal": return Keys.Decimal;
			case "numpadenter": return Keys.Enter;
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
		}

		_log.LogWarning("Unknown key code '{Code}'", code);

		return Keys.None;
	}
}