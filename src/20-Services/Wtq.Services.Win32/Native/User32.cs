using System.Runtime.InteropServices;
using System.Text;

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

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool ShowWindow(nint hWnd, WindowShowStyle nCmdShow);




	[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	static extern int GetClassName(nint hWnd, StringBuilder lpClassName, int nMaxCount);

	public static string GetClassName(nint hWnd)
	{

		var sb = new StringBuilder(256);
		GetClassName(hWnd, sb, 256);
		return sb.ToString();
	}



	[DllImport("oleacc.dll", CharSet = CharSet.Unicode)]
	public static extern int GetProcessHandleFromHwnd(IntPtr hWnd);

	//[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
	//public static extern int GetProcessId(nint processHandle);

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	private static extern int GetWindowTextLength(IntPtr hWnd);

	[DllImport("user32.dll")]
	private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

	// Delegate to filter which windows to include 
	public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	/// <summary> Get the text for the window pointed to by hWnd </summary>
	public static string GetWindowText(IntPtr hWnd)
	{
		int size = GetWindowTextLength(hWnd);
		if (size > 0)
		{
			var builder = new StringBuilder(size + 1);
			GetWindowText(hWnd, builder, builder.Capacity);
			return builder.ToString();
		}

		return String.Empty;
	}

	/// <summary> Find all windows that match the given filter </summary>
	/// <param name="filter"> A delegate that returns true for windows
	///    that should be returned and false for windows that should
	///    not be returned </param>
	public static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter)
	{
		IntPtr found = IntPtr.Zero;
		List<IntPtr> windows = new List<IntPtr>();

		EnumWindows(delegate (IntPtr wnd, IntPtr param)
		{
			//if (filter(wnd, param))
			{
				// only add the windows that pass the filter
				windows.Add(wnd);
			}

			// but return true here so that we iterate all windows
			return true;
		}, IntPtr.Zero);

		return windows;
	}

	/// <summary> Find all windows that contain the given title text </summary>
	/// <param name="titleText"> The text that the window title must contain. </param>
	public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
	{
		return FindWindows(delegate (IntPtr wnd, IntPtr param)
		{
			return GetWindowText(wnd).Contains(titleText);
		});
	}













	[DllImport("user32.dll")]
	static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left;        // x position of upper-left corner
		public int Top;         // y position of upper-left corner
		public int Right;       // x position of lower-right corner
		public int Bottom;      // y position of lower-right corner
	}

	[DllImport("user32.dll", SetLastError = true)]
	static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

	// Callback Declaration
	public delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);
	[DllImport("user32.dll")]
	private static extern int EnumWindows(EnumWindowsCallback callPtr, int lParam);

	public static bool ReportWindow(nint hwnd, int lParam)
	{
		var style = GetWindowLong(hwnd, GWLSTYLE);
		//var isVisible = (style & WS_VISIBLE) == WS_VISIBLE;

		uint processId = 0;
		uint threadId = GetWindowThreadProcessId(hwnd, out processId);

		RECT rt = new RECT();
		bool locationLookupSucceeded = GetWindowRect(hwnd, out rt);
		var area = (rt.Right - rt.Left) * (rt.Bottom - rt.Top);

		Process ownerProcess = Process.GetProcessById((int)processId);
		bool isMainWindow = ownerProcess.MainWindowHandle.Equals(hwnd);
		//Console.WriteLine(string.Format("  Process: {0}{1}", ownerProcess.ProcessName, isMainWindow ? " (MainWindow)" : " (Other Window)"));

		var cl = User32.GetClassName(hwnd);

		var p = new Win32Window()
		{
			ProcessId = processId,
			ThreadId = threadId,
			Process = ownerProcess,
			Rect = new(rt.Left, rt.Top, rt.Right - rt.Left, rt.Bottom - rt.Top),
			Style = style,
			WindowCaption = User32.GetWindowText(hwnd),
			WindowClass = cl,
			WindowHandle = hwnd,
		};

		_procs.Add(p);

		//try
		//{


		//	Console.WriteLine($"{ownerProcess.ProcessName} CLASS:{cl} WINDOW_HANDLE:{hwnd} OWNER_PROCESS:{ownerProcess.Id} SELF_PROCESS:{processId} THREAD_ID:{threadId} IS_MAIN:{isMainWindow} AREA:{area} VISIBLE:{isVisible}");
		//}
		//catch (Exception ex)
		//{
		//	//Console.WriteLine(string.Format("  Unable to lookup Process Information for pid {0}", processId));
		//	//Console.WriteLine(string.Format("    - Exception of type {0} occurred.", ex.GetType().Name));
		//}

		return true;
	}


	private static List<Win32Window> _procs = new();

	public static List<Win32Window> GetWin32Windows()
	{

		_procs.Clear();

		// Have to declare a delegate so that a thunk is created, so that win32 may call us back.
		EnumWindowsCallback callBackFn = new EnumWindowsCallback(ReportWindow);

		EnumWindows(callBackFn, 0);

		return _procs;
	}
}