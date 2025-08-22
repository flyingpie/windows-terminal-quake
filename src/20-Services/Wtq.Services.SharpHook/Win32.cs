using Windows.Win32.UI.Input.KeyboardAndMouse;
using PI = Windows.Win32.PInvoke;
using WKC = Wtq.Input.KeyCode;

namespace Wtq.Services.SharpHook;

public class Win32 : IWin32
{
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

	private static bool IsKeyPressed(VIRTUAL_KEY keyCode) =>
		(PI.GetKeyState((int)keyCode) & 0x800) != 0;
}