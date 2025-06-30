using System.Runtime.InteropServices;

namespace Wtq.Services.Win32.Native;

public static class User32
{
#pragma warning disable CA1707 // Identifiers should not contain underscores // MvdO: In line with MSDN.
#pragma warning disable SA1310 // Field names should not contain underscore

	public const int GWLEXSTYLE = -20;
	public const nint HWNDTOPMOST = -1;
	public const nint HWNDNOTOPMOST = -2;
	public const int LWAALPHA = 0x2;
	public const uint SWPNOMOVE = 0x0002;
	public const uint SWPNOSIZE = 0x0001;
	public const uint TOPMOSTFLAGS = SWPNOMOVE | SWPNOSIZE;
	public const int WmPaint = 0x000F;
	public const int WSEXAPPWINDOW = 0x00040000;
	public const int WSEXLAYERED = 0x80000;
	public const int WSEXTOOLWINDOW = 0x00000080;
	public const byte VK_MENU = 0x12; // Alt key
	public const uint KEYEVENTF_KEYUP = 0x0002;
	public const uint WM_NULL = 0x0000;

#pragma warning restore SA1310 // Field names should not contain underscore
#pragma warning restore CA1707 // Identifiers should not contain underscores

	private static readonly ILogger _log = Log.For(typeof(User32));

	public static void ForcePaint(IntPtr hWnd)
	{
		_ = SendMessage(hWnd, WmPaint, IntPtr.Zero, IntPtr.Zero);
	}

	[DllImport("user32.dll", SetLastError = true)]
	public static extern nint GetDesktopWindow();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern nint GetForegroundWindow();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern nint GetShellWindow();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern int GetWindowLong(nint hWnd, int nIndex);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetWindowRect(nint hwnd, ref Bounds rectangle);

	[DllImport("user32.dll")]
	public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool MoveWindow(nint hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

	[DllImport("User32.dll")]
	public static extern long SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern nint SetForegroundWindow(nint hWnd);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool SetLayeredWindowAttributes(nint hwnd, uint crKey, byte bAlpha, uint dwFlags);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
	public static extern int SetWindowText(IntPtr hWnd, string text);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool ShowWindow(nint hWnd, WindowShowStyle nCmdShow);

	/// <summary>
	/// Synthesizes a keystroke. The system can use such a synthesized keystroke to generate a WM_KEYUP or WM_KEYDOWN message.
	/// The keyboard driver's interrupt handler calls the keybd_event function.
	///
	/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-keybd_event.
	/// </summary>
	[DllImport("user32.dll")]
	public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

	[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	public static extern IntPtr SendMessageTimeout(
		IntPtr hWnd,
		uint Msg,
		UIntPtr wParam,
		IntPtr lParam,
		SendMessageTimeoutFlags fuFlags,
		uint uTimeout,
		out UIntPtr lpdwResult);

	/// <summary>
	/// Usually, we can just call <see cref="SetForegroundWindow"/>, and we're done.<br/>
	/// However, to prevent abuse of this feature, Microsoft implemented a couple rules around setting foreground windows:
	/// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setforegroundwindow#remarks.
	///
	/// Usually, of the second set of criteria (of which we need to hit at least 1), we'd hit this one:
	/// - The calling process received the last input event.
	///
	/// That's because when a hotkey is pressed, WTQ receives an input event, and hence received "the last input event".
	///
	/// But in some other cases, like sending a command to WTQ without pressing a hotkey, none of these criteria might be hit,
	/// and calling <see cref="SetForegroundWindow"/> doesn't do anything.
	///
	/// A trick that apparently is also used by the Chromium team, is to send a synthetic input event, which would
	/// make our process the one with "the last input event".
	/// https://stackoverflow.com/a/13881647.
	/// </summary>
	public static void ForceForegroundWindow(IntPtr hWnd)
	{
		// Attempt the regular method first, simpler and faster.
		SetForegroundWindow(hWnd);

		// Wait for the target window to process a null event, which means the previously sent "become foreground"-event has also been processed.
		// Do this with a timeout, so we don't hang if the target window hangs.
		// https://devblogs.microsoft.com/oldnewthing/20161118-00/?p=94745
		SendMessageTimeout(hWnd, WM_NULL, 0, 0, SendMessageTimeoutFlags.SMTO_NORMAL, uTimeout: 100, out _);

		// If the requested window has become the foreground window, we're done.
		if (GetForegroundWindow() == hWnd)
		{
			return;
		}

		_log.LogWarning("{MethodName} failed, attempting synthetic input event workaround", nameof(SetForegroundWindow));

		// Simulate Alt key press and release.
		keybd_event(VK_MENU, 0, 0, UIntPtr.Zero);
		keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

		Thread.Sleep(20); // Give Windows a moment to process input events.

		// We should now be the app with the most recent input event, so we should be allowed to set the foreground window.
		SetForegroundWindow(hWnd);

		// Wait for the above event to be processed again.
		SendMessageTimeout(hWnd, WM_NULL, 0, 0, SendMessageTimeoutFlags.SMTO_NORMAL, uTimeout: 100, out _);

		if (GetForegroundWindow() == hWnd)
		{
			_log.LogDebug("Synthetic input event workaround successful");
		}
		else
		{
			_log.LogWarning("Synthetic input event workaround failed, window may not have focus");
		}
	}
}