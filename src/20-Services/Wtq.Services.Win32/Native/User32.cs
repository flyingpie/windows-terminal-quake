using System.Runtime.InteropServices;

namespace Wtq.Services.Win32.Native;

public static class User32
{
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
}