using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Wtq.Services.SharpHook;

public static class User32
{
	public static byte VK_LSHIFT = 0xA0;
	public static byte VK_RSHIFT = 0xA1;
	public static byte VK_LCONTROL = 0xA2;
	public static byte VK_RCONTROL = 0xA3;
	public static byte VK_LMENU = 0xA4;
	public static byte VK_RMENU = 0xA5;

	public static string? KeyCodeToUnicode(uint key)
	{
		// TODO: Keyboard layout changes require WTQ restart atm.
		KeysConverter converter = new KeysConverter();
		var res = converter.ConvertToString((System.Windows.Forms.Keys)key);
		Console.WriteLine($"RES:{res}");

		// TODO: When "Control" is pressed, we're not getting back key characters. Kind of understandable, but messes up the registration.
		// Remove "Control" from the keyboard state?
		byte[] keyboardState = new byte[256];
		bool keyboardStateStatus = GetKeyboardState(keyboardState);

		Console.WriteLine($"STATE:{Convert.ToHexString(keyboardState)}");

		if (!keyboardStateStatus)
		{
			return null;
		}

		uint virtualKeyCode = (uint)key;
		uint scanCode = MapVirtualKey(virtualKeyCode, 0);
		IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

		Console.WriteLine($"LAYOUT:{inputLocaleIdentifier}");

		StringBuilder result = new StringBuilder();
		ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0, inputLocaleIdentifier);

		return result.ToString();
	}

	[DllImport("user32.dll")]
	static extern bool GetKeyboardState(byte[] lpKeyState);

	[DllImport("user32.dll")]
	static extern uint MapVirtualKey(uint uCode, uint uMapType);

	[DllImport("user32.dll")]
	static extern IntPtr GetKeyboardLayout(uint idThread);

	[DllImport("user32.dll")]
	static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);
}