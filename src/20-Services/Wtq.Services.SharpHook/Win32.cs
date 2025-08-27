using System.ComponentModel.DataAnnotations;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using PI = Windows.Win32.PInvoke;
using WKC = Wtq.Input.KeyCode;

namespace Wtq.Services.SharpHook;

public class Win32 : IWin32
{
	private readonly ILogger _log = Log.For<Win32>();

	/// <inheritdoc/>
	public KeySequence GetKeySequence(WKC keyCode, ushort rawKeyCode)
	{
		// Attempt to translate the virtual key code to a key character (may return null).
		var keyChar = KeyCodeToKeyChar(rawKeyCode)
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

		if (IsAltPressed())
		{
			mod2 |= KeyModifiers.Alt;
		}

		if (IsControlPressed())
		{
			mod2 |= KeyModifiers.Control;
		}

		if (IsShiftPressed())
		{
			mod2 |= KeyModifiers.Shift;
		}

		if (IsSuperPressed())
		{
			mod2 |= KeyModifiers.Super;
		}

		if (keyCode.IsNumpad())
		{
			mod2 |= KeyModifiers.Numpad;
		}

		return mod2;
	}

	/// <summary>
	/// Returns whether the ALT key is currently held down.
	/// </summary>
	private bool IsAltPressed() =>
		IsKeyPressed(VIRTUAL_KEY.VK_MENU);

	/// <summary>
	/// Returns whether the CONTROL key is currently held down.
	/// </summary>
	private bool IsControlPressed() =>
		IsKeyPressed(VIRTUAL_KEY.VK_CONTROL);

	/// <summary>
	/// Returns whether the SHIFT key is currently held down.
	/// </summary>
	private bool IsShiftPressed() =>
		IsKeyPressed(VIRTUAL_KEY.VK_SHIFT);

	/// <summary>
	/// Returns whether the SUPER (or "Meta" or "Windows") key is currently held down.
	/// </summary>
	private bool IsSuperPressed() =>
		IsKeyPressed(VIRTUAL_KEY.VK_LWIN) || IsKeyPressed(VIRTUAL_KEY.VK_RWIN);

	/// <summary>
	/// Attempts to convert the specified virtual <paramref name="keyCode"/> to a UTF8 character representation,
	/// taking the current keyboard layout into account.
	/// </summary>
	private string? KeyCodeToKeyChar(uint keyCode)
	{
		var keyChar = (char)PI.MapVirtualKey(keyCode, MAP_VIRTUAL_KEY_TYPE.MAPVK_VK_TO_CHAR);

		_log.LogTrace("Mapped key code '{KeyCode}' to character '{KeyChar}'", keyCode, keyChar);

		return keyChar.ToString();
	}

	private static bool IsKeyPressed(VIRTUAL_KEY keyCode) =>
		(PI.GetKeyState((int)keyCode) & 0x800) != 0;
}