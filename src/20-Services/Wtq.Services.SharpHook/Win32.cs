using Windows.Win32.UI.Input.KeyboardAndMouse;
using PI = Windows.Win32.PInvoke;

namespace Wtq.Services.SharpHook;

public class Win32 : IWin32
{
	private readonly ILogger _log = Log.For<Win32>();

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

		// Only include Shift key if it's currently down.
		// We could ask for the actual keyboard state, but we don't want to include modifiers like CONTROL, as they throw off key conversion.
		// For example, SHIFT+1 should return "!", but SHIFT+CTRL+1 suddendly returns "1". We're interested in the shifted version ("!") in that case.
		if (IsShiftPressed())
		{
			keyState[(int)VIRTUAL_KEY.VK_SHIFT] = 0x80;
		}

		// Get the scan code for the specified key.
		var scanCode = PI.MapVirtualKey(keyCode, 0);

		// Get the currently active keyboard layout.
		// TODO: Keyboard layout changes require WTQ restart it seems.
		//var threadId = GetThreadId();
		var layout = PI.GetKeyboardLayout_SafeHandle(0);

		// Build a buffer for the resulting UTF8 string.
		var buffer = new Span<char>(new char[5]);

		// Try to convert the virtual key code.
		var length = PI.ToUnicodeEx(
			wVirtKey: keyCode,
			wScanCode: scanCode,
			lpKeyState: keyState,
			pwszBuff: buffer,
			wFlags: 0,
			dwhkl: layout);

		//_log.LogInformation("LAYOUT:{Layout} BUFFER:{Buffer} ({Length})", layout.DangerousGetHandle(), buffer[..5].ToString(), length);

		// Pull the relevant part out of the buffer (as specified by the returned "length").
		var result = buffer.ToString().Trim('\0').Trim();

		// The result could still be empty, e.g. for the "Tab" character, which returns \t.
		if (string.IsNullOrWhiteSpace(result))
		{
			return null;
		}

		// Now we can return the actual UTF8 representation.
		_log.LogInformation("Got character '{Char}'", result);
		return result;
	}

	//private unsafe uint GetThreadId()
	//{
	//	var fg = PI.GetForegroundWindow();
	//	if (fg == 0)
	//	{
	//		_log.LogWarning("Foreground window NULL"); // TODO: Fallback to current thread?
	//		return 0;
	//	}

	//	var threadId = PI.GetWindowThreadProcessId(fg, null);
	//	if (threadId == 0)
	//	{
	//		_log.LogWarning("FG thread NULL");
	//	}

	//	return threadId;
	//}

	private static bool IsKeyPressed(VIRTUAL_KEY keyCode) =>
		(PI.GetKeyState((int)keyCode) & 0x800) != 0;
}