using System;
using System.Runtime.InteropServices;

namespace Wtq.Win32.Native;

public static class User32
{
	public const int GWL_EX_STYLE = -20;

	public const int LWA_ALPHA = 0x2;

	public const int WS_EX_APPWINDOW = 0x00040000;
	public const int WS_EX_LAYERED = 0x80000;
	public const int WS_EX_TOOLWINDOW = 0x00000080;

	public static readonly nint HWND_TOPMOST = -1;
	public const uint SWP_NOSIZE = 0x0001;
	public const uint SWP_NOMOVE = 0x0002;
	public const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

	private const int WmPaint = 0x000F;

	[StructLayout(LayoutKind.Sequential)]
	internal struct MSG
	{
		internal IntPtr hwnd;
		internal uint message;
		internal uint wParam;
		internal uint lParam;
		internal uint time;
		internal int ptX;
		internal int ptY;
	}

	[DllImport("User32.dll")]
	public static extern Int64 SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

	public static void ForcePaint(IntPtr hWnd)
	{
		SendMessage(hWnd, WmPaint, IntPtr.Zero, IntPtr.Zero);
	}

	[DllImport("user32.dll")]
	public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);


	[DllImport("user32.dll", SetLastError = true)]
	public static extern nint GetForegroundWindow();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern nint GetDesktopWindow();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool SetLayeredWindowAttributes(nint hwnd, uint crKey, byte bAlpha, uint dwFlags);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern nint GetShellWindow();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern int GetWindowLong(nint hWnd, int nIndex);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetWindowRect(nint hwnd, ref Bounds rectangle);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool MoveWindow(nint hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern nint SetForegroundWindow(nint hWnd);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

	[DllImport("user32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool ShowWindow(nint hWnd, WindowShowStyle nCmdShow);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool UnregisterHotKey(nint hWnd, int id);

	//[DllImport("User32", ExactSpelling = true, EntryPoint = "GetMessageW", SetLastError = true)]
	//[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	//internal static extern unsafe bool GetMessage(MSG* lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

	//[DllImport("User32", ExactSpelling = true)]
	//[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	//internal static extern unsafe int TranslateMessage(MSG* lpMsg);

	//[DllImport("User32", ExactSpelling = true, EntryPoint = "DispatchMessageW")]
	//[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	//internal static extern unsafe nint DispatchMessage(MSG* lpMsg);

	//internal static unsafe void RunLoop(CancellationToken cancellationToken)
	//{
	//	MSG msg = default;
	//	var lpMsg = &msg;
	//	while (!cancellationToken.IsCancellationRequested && GetMessage(lpMsg, default, 0, 0))
	//	{
	//		TranslateMessage(lpMsg);
	//		DispatchMessage(lpMsg);
	//	}
	//}
}