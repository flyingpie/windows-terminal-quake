using System.ComponentModel.DataAnnotations;
using WKC = Wtq.Input.KeyCode;

namespace Wtq.Services.SharpHook.Input;

/// <inheritdoc cref="IWin32KeyService"/>
public class Win32KeyService(IWin32 win32) : IWin32KeyService
{
	private readonly IWin32 _win32 = Guard.Against.Null(win32);

	/// <inheritdoc/>
	public KeySequence GetKeySequence(WKC keyCode, ushort rawKeyCode)
	{
		// Attempt to translate the virtual key code to a key character (may return null).
		var keyChar = _win32.KeyCodeToKeyChar(rawKeyCode)
			?? keyCode.GetAttribute<DisplayAttribute>()?.Name ?? keyCode.ToString();

		var mod = GetModifiers(keyCode);

		return new KeySequence(mod, keyChar, keyCode);
	}

	/// <summary>
	/// Returns the set of <see cref="KeyModifiers"/> that are currently active.<br/>
	/// Also includes the <see cref="KeyModifiers.Numpad"/> modifier, if the specified <paramref name="keyCode"/> contains a numpad key.
	/// </summary>
	private KeyModifiers GetModifiers(WKC keyCode)
	{
		var mod2 = KeyModifiers.None;

		if (_win32.IsAltPressed())
		{
			mod2 |= KeyModifiers.Alt;
		}

		if (_win32.IsControlPressed())
		{
			mod2 |= KeyModifiers.Control;
		}

		if (_win32.IsShiftPressed())
		{
			mod2 |= KeyModifiers.Shift;
		}

		if (_win32.IsSuperPressed())
		{
			mod2 |= KeyModifiers.Super;
		}

		if (keyCode.IsNumpad())
		{
			mod2 |= KeyModifiers.Numpad;
		}

		return mod2;
	}
}