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

		//foreach (var k in Enum.GetValues<System.Windows.Forms.Keys>())
		//{
		//	var res2 = converter.ConvertToString(k);
		//	Console.WriteLine($"WINFORMS:{k} => {res2}");
		//}

//		Console.WriteLine($"RES:{res}");

		// TODO: When "Control" is pressed, we're not getting back key characters. Kind of understandable, but messes up the registration.
		// Remove "Control" from the keyboard state?
		byte[] keyboardState = new byte[256];
		bool keyboardStateStatus = GetKeyboardState(keyboardState);

//		Console.WriteLine($"STATE:{Convert.ToHexString(keyboardState)}");

		if (!keyboardStateStatus)
		{
			return res;
		}

		uint virtualKeyCode = (uint)key;
		uint scanCode = MapVirtualKey(virtualKeyCode, 0);
		IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

//		Console.WriteLine($"LAYOUT:{inputLocaleIdentifier}");

		StringBuilder result = new StringBuilder();
		ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0, inputLocaleIdentifier);

		var res2 = result.ToString();

		if (!string.IsNullOrWhiteSpace(res2))
		{
			Console.WriteLine($"KEYCODE_TO_KEYCHAR from ToUnicodeEx: {res2}");
			return res2;
		}

		Console.WriteLine($"KEYCODE_TO_KEYCHAR from KeysConverter: {res}");
		return res;
	}

	const int VK_MENU = 0x12; // Alt
	const int VK_CONTROL = 0x11; // Control
	const int VK_SHIFT = 0x10; // Shift
	const int VK_LWIN = 0x5b;
	const int VK_RWIN = 0x5c;

	public static string KeyCodeToKeyChar(uint keyCode)
	{
		var keyState = new byte[256];

		// Only include Shift key if it's currently down
		if ((GetKeyState(VK_SHIFT) & 0x8000) != 0)
		{
			keyState[VK_SHIFT] = 0x80;
		}

		uint scanCode = MapVirtualKey(keyCode, 0);
		StringBuilder sb = new StringBuilder(5);
		IntPtr layout = GetKeyboardLayout(0);

		int result = ToUnicodeEx(
			keyCode,
			scanCode,
			keyState,
			sb,
			sb.Capacity,
			0,
			layout);

		Console.WriteLine($"KEYCODE_TO_KEYCHAR from KeyCodeToKeyChar: {sb.ToString()}");

		return result > 0 ? sb.ToString() : string.Empty;
	}

	public static bool IsAltPressed() => IsKeyPressed(VK_MENU);

	public static bool IsControlPressed() => IsKeyPressed(VK_CONTROL);

	public static bool IsShiftPressed() => IsKeyPressed(VK_SHIFT);

	public static bool IsSuperPressed() => IsKeyPressed(VK_LWIN) || IsKeyPressed(VK_RWIN);

	public static bool IsKeyPressed(int keyCode) => (GetKeyState(keyCode) & 0x800) != 0;

	[DllImport("user32.dll")]
	public static extern short GetKeyState(int nVirtKey);

	[DllImport("user32.dll")]
	static extern bool GetKeyboardState(byte[] lpKeyState);

	[DllImport("user32.dll")]
	static extern uint MapVirtualKey(uint uCode, uint uMapType);

	[DllImport("user32.dll")]
	static extern IntPtr GetKeyboardLayout(uint idThread);

	[DllImport("user32.dll")]
	static extern int ToUnicodeEx(
		uint wVirtKey,
		uint wScanCode,
		byte[] lpKeyState,
		[Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff,
		int cchBuff,
		uint wFlags,
		IntPtr dwhkl);
}