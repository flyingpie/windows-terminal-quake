namespace Wtq.Services.KWin.Input;

[SuppressMessage(
	"StyleCop.CSharp.DocumentationRules",
	"SA1649:File name should match first type name",
	Justification = "MvdO: Naming convention."
)]
public static class KeySequenceExtensions
{
	// Qt::KeyboardModifier bits, see https://doc.qt.io/qt-6/qt.html#KeyboardModifier-enum
	private const int QtShiftModifier = 0x02000000;
	private const int QtControlModifier = 0x04000000;
	private const int QtAltModifier = 0x08000000;
	private const int QtMetaModifier = 0x10000000;
	private const int QtKeypadModifier = 0x20000000;

	// Qt::Key_F1, the function keys are consecutive from here (F1 => 0x01000030, F2 => 0x01000031, etc.).
	private const int QtKeyFunctionBase = 0x01000030;

	private static readonly ILogger _log = Log.For(typeof(KeySequenceExtensions));

	/// <summary>
	/// Converts this <see cref="KeySequence"/> into the integer representation of a Qt key sequence,
	/// i.e. a Qt::Key combined with Qt::KeyboardModifiers in the high bits.<br/>
	/// <br/>
	/// This is the format used by KGlobalAccel, e.g.
	/// <c>org.kde.KGlobalAccel.setShortcutKeys</c>.
	/// </summary>
	public static int ToQtKeyCombination(this KeySequence sequence)
	{
		// Determine whether the "Shift" key is required.
		// I.e., with sequence "Shift+F1", it _is_ required, as the actual key isn't affected by the shift.
		// But with "Ctrl+Shift+1", it _is not_ required, as it will be bound as "Ctrl+!" (on US keyboards).
		var mod = sequence.Modifiers;
		if (sequence.IsShiftImplied())
		{
			_log.LogInformation(
				"Key sequence '{Sequence}' contains explicit SHIFT, which is implied by character '{Char}', omitting from Qt key combination",
				sequence,
				sequence.KeyChar
			);

			mod ^= KeyModifiers.Shift;
		}

		// Qt uses key characters, so prefer that one if we have it right in the settings.
		var key = sequence.HasKeyChar ? sequence.KeyChar.ToQtKey() : sequence.KeyCode.ToQtKey();

		return key | ToQtModifiers(mod);
	}

	private static int ToQtModifiers(this KeyModifiers modifiers)
	{
		var result = 0;

		if (modifiers.HasFlag(KeyModifiers.Shift))
		{
			result |= QtShiftModifier;
		}

		if (modifiers.HasFlag(KeyModifiers.Control))
		{
			result |= QtControlModifier;
		}

		if (modifiers.HasFlag(KeyModifiers.Alt))
		{
			result |= QtAltModifier;
		}

		if (modifiers.HasFlag(KeyModifiers.Super))
		{
			result |= QtMetaModifier;
		}

		if (modifiers.HasFlag(KeyModifiers.Numpad))
		{
			result |= QtKeypadModifier;
		}

		return result;
	}

	private static int ToQtKey(this string keyChar)
	{
		// If the key character happens to refer to a KeyCode name (e.g. "F1", "Escape"), map that.
		if (EnumUtils.TryParse<KeyCode>(keyChar, ignoreCase: true, out var code))
		{
			var keyCode = code.ToQtKey();
			if (keyCode != 0)
			{
				return keyCode;
			}
		}

		// Otherwise, for a single character, the Qt::Key value matches the (upper-cased) character code.
		if (keyChar.Length == 1)
		{
			return char.ToUpperInvariant(keyChar[0]);
		}

		_log.LogWarning("Could not map key character '{KeyChar}' to a Qt key", keyChar);

		return 0;
	}

	private static int ToQtKey(this KeyCode keyCode)
	{
		// Function keys, e.g. "F1" => "Qt::Key_F1".
		if (keyCode is >= KeyCode.F1 and <= KeyCode.F24)
		{
			return QtKeyFunctionBase + ((int)keyCode - (int)KeyCode.F1);
		}

		// Numpad digits, e.g. "Numpad1" => "Qt::Key_1" (the "Num" modifier distinguishes these from the main row).
		if (keyCode is >= KeyCode.Numpad0 and <= KeyCode.Numpad9)
		{
			return '0' + ((int)keyCode - (int)KeyCode.Numpad0);
		}

		// @formatter:off
#pragma warning disable SA1025
#pragma warning disable SA1027

		switch (keyCode)
		{
			// Letters, digits and the main row already match their Qt::Key values, so those are handled by the default case below.

			// The Most Popular Keys
			case KeyCode.Escape:
				return 0x01000000;
			case KeyCode.Tab:
				return 0x01000001;
			case KeyCode.Backspace:
				return 0x01000003;
			case KeyCode.Return:
				return 0x01000004;
			case KeyCode.Insert:
				return 0x01000006;
			case KeyCode.Delete:
				return 0x01000007;
			case KeyCode.Pause:
				return 0x01000008;
			case KeyCode.PrintScreen:
				return 0x01000009;
			case KeyCode.Space:
				return 0x20;
			case KeyCode.ContextMenu:
				return 0x01000055;

			// Cursor movement
			case KeyCode.Home:
				return 0x01000010;
			case KeyCode.End:
				return 0x01000011;
			case KeyCode.ArrowLeft:
				return 0x01000012;
			case KeyCode.ArrowUp:
				return 0x01000013;
			case KeyCode.ArrowRight:
				return 0x01000014;
			case KeyCode.ArrowDown:
				return 0x01000015;
			case KeyCode.PageUp:
				return 0x01000016;
			case KeyCode.PageDown:
				return 0x01000017;

			// Modifiers
			case KeyCode.ShiftLeft:
			case KeyCode.ShiftRight:
				return 0x01000020;
			case KeyCode.ControlLeft:
			case KeyCode.ControlRight:
				return 0x01000021;
			case KeyCode.SuperLeft:
			case KeyCode.SuperRight:
				return 0x01000022;
			case KeyCode.AltLeft:
			case KeyCode.AltRight:
				return 0x01000023;

			// Lock keys
			case KeyCode.CapsLock:
				return 0x01000024;
			case KeyCode.NumLock:
				return 0x01000025;
			case KeyCode.ScrollLock:
				return 0x01000026;

			// Browser
			case KeyCode.BrowserBack:
				return 0x01000061;
			case KeyCode.BrowserForward:
				return 0x01000062;
			case KeyCode.BrowserStop:
				return 0x01000063;
			case KeyCode.BrowserRefresh:
				return 0x01000064;
			case KeyCode.BrowserHome:
				return 0x01000090;
			case KeyCode.BrowserFavorites:
				return 0x01000091;
			case KeyCode.BrowserSearch:
				return 0x01000092;

			// Volume
			case KeyCode.VolumeDown:
				return 0x01000070;
			case KeyCode.VolumeMute:
				return 0x01000071;
			case KeyCode.VolumeUp:
				return 0x01000072;

			// Media
			case KeyCode.MediaStop:
				return 0x01000081;
			case KeyCode.MediaPreviousTrack:
				return 0x01000082;
			case KeyCode.MediaNextTrack:
				return 0x01000083;
			case KeyCode.MediaPlayPause:
				return 0x01000086;

			// Numpad operators (the "Num" modifier distinguishes these from the main row).
			case KeyCode.NumpadMultiply:
				return 0x2A;
			case KeyCode.NumpadAdd:
				return 0x2B;
			case KeyCode.NumpadSeparator:
				return 0x2C;
			case KeyCode.NumpadSubtract:
				return 0x2D;
			case KeyCode.NumpadDecimal:
				return 0x2E;
			case KeyCode.NumpadDivide:
				return 0x2F;

			// OEM keys (mapped to their US layout characters; prefer the key character when available).
			case KeyCode.Oem1:
				return 0x3B;
			case KeyCode.OemPlus:
				return 0x2B;
			case KeyCode.OemComma:
				return 0x2C;
			case KeyCode.OemMinus:
				return 0x2D;
			case KeyCode.OemPeriod:
				return 0x2E;
			case KeyCode.Oem2:
				return 0x2F;
			case KeyCode.Oem3:
				return 0x60;
			case KeyCode.Oem4:
				return 0x5B;
			case KeyCode.Oem5:
				return 0x5C;
			case KeyCode.Oem6:
				return 0x5D;
			case KeyCode.Oem7:
				return 0x27;
			case KeyCode.Oem102:
				return 0x5C;

			// Letters ("A" => 0x41), digits ("D1" => 0x31) and other ASCII-range keys already match their Qt::Key value.
			default:
				return (int)keyCode;
		}

#pragma warning restore SA1027
#pragma warning restore SA1025
		// @formatter:on
	}
}
