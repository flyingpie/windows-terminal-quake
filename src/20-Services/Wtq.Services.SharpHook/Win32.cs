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
	public KeyModifiers GetModifiers(WKC keyCode)
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

	public bool IsAltPressed() =>
		IsKeyPressed(VIRTUAL_KEY.VK_MENU);

	public bool IsControlPressed() =>
		IsKeyPressed(VIRTUAL_KEY.VK_CONTROL);

	public bool IsShiftPressed() =>
		IsKeyPressed(VIRTUAL_KEY.VK_SHIFT);

	public bool IsSuperPressed() =>
		IsKeyPressed(VIRTUAL_KEY.VK_LWIN) || IsKeyPressed(VIRTUAL_KEY.VK_RWIN);

	/// <summary>
	/// Attempts to convert the specified virtual <paramref name="keyCode"/> to a UTF8 character representation,
	/// taking the current keyboard layout into account.
	/// </summary>
	public string? KeyCodeToKeyChar(uint keyCode)
	{


		// Build a key state.
		// The key state represents the state of each virtual key.
		// The ToUnicodeEx function uses this to determine whether to return "A" or "a", and "1" or "!", for example.
		var keyState = new byte[256];

		if (IsControlPressed())
		{
			//keyState[(int)VIRTUAL_KEY.VK_CONTROL] = 0x80;
		}

		// Only include Shift key if it's currently down.
		// We could ask for the actual keyboard state, but we don't want to include modifiers like CONTROL, as they throw off key conversion.
		// For example, SHIFT+1 should return "!", but SHIFT+CTRL+1 suddendly returns "1". We're interested in the shifted version ("!") in that case.
		if (IsShiftPressed())
		{
			keyState[(int)VIRTUAL_KEY.VK_SHIFT] = 0x80;
		}

		// Get the scan code for the specified key.
		var scanCode = PI.MapVirtualKey(keyCode, 0);

		var x = (char)PI.MapVirtualKey(keyCode, MAP_VIRTUAL_KEY_TYPE.MAPVK_VK_TO_CHAR); //& 0x00FFFFFF;
		_log.LogWarning($"CHAR:{x}");

		//var y = (char)PI.MapVirtualKey(keyCode, MAP_VIRTUAL_KEY_TYPE.MAPVK_VK_TO_CHAR); //& 0x00FFFFFF;
		//_log.LogWarning($"OEM3:{y}");

		return x.ToString();

		// Get the currently active keyboard layout.
		// TODO: Keyboard layout changes require WTQ restart it seems.
		//var layout = PI.GetKeyboardLayout_SafeHandle(0); // Cache this?

		// Build a buffer for the resulting UTF8 string.
		//var buffer = new Span<char>(new char[5]);

		// Try to convert the virtual key code.

		//var length = PI.ToUnicodeEx(
		//	wVirtKey: keyCode,
		//	wScanCode: scanCode,
		//	lpKeyState: keyState,
		//	pwszBuff: buffer,
		//	wFlags: 0,
		//	dwhkl: layout);

		//var length = PI.ToUnicode(wVirtKey: keyCode, wScanCode: scanCode, lpKeyState: keyState, pwszBuff: buffer, wFlags: 0);

		//_log.LogWarning($"ToUnicodeEx(mVirtKey:{keyCode}, wScanCode:{scanCode}, lpKeyState:-, pwszBuff:-, dwhkl:{layout.DangerousGetHandle()}) => Length:{length}");

		//var result = buffer.ToString().Trim('\0').Trim();

		//_log.LogWarning($"ToUnicodeEx(mVirtKey:{keyCode}, wScanCode:{scanCode}, lpKeyState:-, pwszBuff:-) => Length:{length} [RESULT:{result}]");

		//// The result could still be empty, e.g. for the "Tab" character, which returns \t.
		//if (string.IsNullOrWhiteSpace(result))
		//{
		//	return null;
		//}

		//// Now we can return the actual UTF8 representation.
		//return result;
		return null;
	}

	private static bool IsKeyPressed(VIRTUAL_KEY keyCode) =>
		(PI.GetKeyState((int)keyCode) & 0x800) != 0;
}