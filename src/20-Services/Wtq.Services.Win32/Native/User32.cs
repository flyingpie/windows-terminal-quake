#pragma warning disable CA1416 // Validate platform compatibility

using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Wtq.Services.Win32.Native;

public static class User32
{
	public const int GWLSTYLE = -16;
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
	public const long WS_VISIBLE = 0x10000000L;
	public const long WS_CAPTION = 0x00C00000L;

	public static void ForcePaint(IntPtr hWnd)
	{
		SendMessage(hWnd, WmPaint, IntPtr.Zero, IntPtr.Zero);
	}

	//[DllImport("user32.dll", SetLastError = true)]
	//public static extern nint GetDesktopWindow();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern nint GetForegroundWindow();

	//[DllImport("user32.dll", SetLastError = true)]
	//public static extern nint GetShellWindow();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern int GetWindowLong(nint hWnd, int nIndex);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetWindowRect(nint hwnd, ref Bounds rectangle);

	[DllImport("user32.dll")]
	public static extern IntPtr GetWindowThreadProcessId(nint hWnd, out nint processId);

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

	[DllImport("user32.dll", SetLastError = true)]
	public static extern int SetWindowText(IntPtr hWnd, string text);

	private static unsafe BOOL ReportWindow(HWND windowHandle, LPARAM lParam)
	{
		var style = Windows.Win32.PInvoke.GetWindowLong(windowHandle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

		uint processId = 0;
		uint threadId = Windows.Win32.PInvoke.GetWindowThreadProcessId(windowHandle, &processId);

		if (threadId == 0 || processId == 0)
		{
			// TODO: Log
			return true;
		}

		if (!Windows.Win32.PInvoke.GetWindowRect(windowHandle, out var rt))
		{
			// TODO: Log
			return true;
		}

		var area = (rt.right - rt.left) * (rt.bottom - rt.top);

		var ownerProcess = Process.GetProcessById((int)processId);
		bool isMainWindow = ownerProcess.MainWindowHandle.Equals(windowHandle);

		var xx = new Span<char>(new char[256]);
		var ccx = Windows.Win32.PInvoke.GetClassName(windowHandle, xx);
		var xxz = xx[..ccx].ToString();

		var p = new Win32Window(ownerProcess)
		{
			ProcessId = processId,
			ThreadId = threadId,
			Rect = new(rt.left, rt.top, rt.right - rt.left, rt.bottom - rt.top),
			Style = style,
			WindowClass = xxz,
			WindowHandle = windowHandle,
		};

		_procs.Add(p);

		return true;
	}

	private static List<Win32Window> _procs = new();

	public static List<Win32Window> GetWin32Windows()
	{
		_procs.Clear();

		Windows.Win32.PInvoke.EnumWindows(ReportWindow, 0); // TODO: Pass result collection as lparam

		return _procs;
	}
}