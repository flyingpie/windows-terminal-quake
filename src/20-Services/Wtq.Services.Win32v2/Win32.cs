#pragma warning disable CA1416 // Validate platform compatibility

using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;
using PI = Windows.Win32.PInvoke;

namespace Wtq.Services.Win32v2;

public class Win32 : IWin32
{
#pragma warning disable SA1310 // MvdO: Naming kept consistent with MSDN.
	private const int HWND_NOTOPMOST = -2;
	private const int HWND_TOPMOST = -1;

	private const int WS_EX_LAYERED = 0x80000;
#pragma warning restore SA1310

	private readonly ILogger _log = Log.For<Win32>();

	private readonly List<Win32Window> _procs = new();

	/// <summary>
	/// Returns the id of the process that currently has focus.<br/>
	/// Returns null if no process was found.
	/// </summary>
	public unsafe uint? GetForegroundProcessId()
	{
		var windowHandle = PI.GetForegroundWindow();

		uint processId;
		var threadId = PI.GetWindowThreadProcessId(windowHandle, &processId);

		if (threadId <= 0)
		{
			_log.LogWarning("{Method} Did not find a foreground process", nameof(GetForegroundProcessId));
			return null;
		}

		_log.LogDebug("{Method} Got foreground process with id '{ProcessId}' (and thread id '{ThreadId}')", nameof(GetForegroundProcessId), processId, threadId);
		return processId;
	}

	/// <summary>
	/// Returns the <see cref="Rectangle"/> describing the window with the specified <paramref name="windowHandle"/>.<br/>
	/// Throws an exception if no rectangle could be obtained.
	/// </summary>
	public Rectangle GetWindowRect(nint windowHandle)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		if (!PI.GetWindowRect((HWND)windowHandle, out var rect))
		{
			throw new InvalidOperationException($"Could not get window rect for window with handle '{windowHandle}'.");
		}

		var res = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		_log.LogDebug("{Method} Got window rect '{Rectangle}' for window with handle '{WindowHandle}'", nameof(GetWindowRect), res, windowHandle);

		return res;
	}

	public string? GetWindowTitle(nint windowHandle)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		var xx = new Span<char>(new char[256]);
		var ccx = PI.GetWindowText((HWND)windowHandle, xx);
		var xxz = xx[..ccx].ToString();

		return xxz;
	}

	/// <summary>
	/// Sets the position and size of the window with the specified <paramref name="windowHandle"/>.
	/// </summary>
	public void MoveWindow(nint windowHandle, Rectangle rectangle)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		if (!PI.MoveWindow((HWND)windowHandle, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, true))
		{
			throw new InvalidOperationException($"Could not set size and position to '{rectangle}' of window with handle '{windowHandle}'.");
		}
	}

	public List<Win32Window> GetWindows()
	{
		_procs.Clear();

		PI.EnumWindows(ReportWindow, 0); // TODO: Pass result collection as lparam

		return _procs;
	}

	public void SetAlwaysOnTop(nint windowHandle, bool isAlwaysOnTop)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		var insertBefore = isAlwaysOnTop ? HWND_TOPMOST : HWND_NOTOPMOST;
		var flags =
			SET_WINDOW_POS_FLAGS.SWP_NOMOVE | // Ignore the positional parameters.
			SET_WINDOW_POS_FLAGS.SWP_NOSIZE // Ignore the size parameters.
		;

		if (!PI.SetWindowPos(hWnd: (HWND)windowHandle, hWndInsertAfter: (HWND)insertBefore, X: 0, Y: 0, cx: 0, cy: 0, uFlags: flags))
		{
			throw new InvalidOperationException($"Could not set 'Always On Top' to '{isAlwaysOnTop}' of window with handle '{windowHandle}'.");
		}
	}

	public void SetForegroundWindow(nint windowHandle)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		PI.SetForegroundWindow((HWND)windowHandle);
		PI.SendMessage((HWND)windowHandle, PI.WM_PAINT, 0, 0);
	}

	public void SetWindowTitle(nint windowHandle, string title)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		PI.SetWindowText((HWND)windowHandle, title);
	}

	public void SetWindowTransparency(nint windowHandle, int transparency)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		// Convert 0-100 alpha to 0-255.
		var alpha = (byte)Math.Ceiling(255f / 100f * transparency);

		// Get original window properties
		var props = PI.GetWindowLong((HWND)windowHandle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);

		// Add "WS_EX_LAYERED"-flag (required for transparency).
		PI.SetWindowLong((HWND)windowHandle, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, props | WS_EX_LAYERED);

		// Set transparency
		if (!PI.SetLayeredWindowAttributes((HWND)windowHandle, crKey: (COLORREF)0, bAlpha: alpha, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_ALPHA))
		{
			throw new InvalidOperationException($"Could not set transparency to '{transparency}' of window with handle '{windowHandle}'.");
		}
	}

	private unsafe BOOL ReportWindow(HWND windowHandle, LPARAM lParam)
	{
		var style = PI.GetWindowLong(windowHandle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

		uint processId = 0;
		var threadId = PI.GetWindowThreadProcessId(windowHandle, &processId);

		if (threadId == 0 || processId == 0)
		{
			// TODO: Log
			return true;
		}

		if (!PI.GetWindowRect(windowHandle, out var rt))
		{
			// TODO: Log
			return true;
		}

		var ownerProcess = Process.GetProcessById((int)processId);
		var isMainWindow = ownerProcess.MainWindowHandle.Equals(windowHandle);

		var xx = new Span<char>(new char[256]);
		var ccx = PI.GetClassName(windowHandle, xx);
		var xxz = xx[..ccx].ToString();

		var windowTitle = GetWindowTitle(windowHandle);

		var p = new Win32Window(ownerProcess)
		{
			IsMainWindow = isMainWindow,
			ProcessId = processId,
			Rect = new(rt.left, rt.top, rt.right - rt.left, rt.bottom - rt.top),
			Style = style,
			ThreadId = threadId,
			WindowCaption = windowTitle,
			WindowClass = xxz,
			WindowHandle = windowHandle,
		};

		_procs.Add(p);

		return true;
	}
}