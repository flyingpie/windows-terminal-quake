#pragma warning disable CA1416 // Validate platform compatibility

using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;
using PI = Windows.Win32.PInvoke;

namespace Wtq.Services.Win32v2;

/// <inheritdoc cref="IWin32"/>
public class Win32 : IWin32
{

#pragma warning disable SA1310 // MvdO: Naming kept consistent with MSDN.

	private const int HWND_NOTOPMOST = -2;
	private const int HWND_TOPMOST = -1;
	private const uint WM_NULL = 0x0000;
	private const int WS_EX_LAYERED = 0x80000;

#pragma warning restore SA1310

	private readonly ILogger _log = Log.For<Win32>();

	/// <inheritdoc/>
	public unsafe nint? GetForegroundWindowHandle()
	{
		var windowHandle = PI.GetForegroundWindow();

		if (windowHandle <= 0)
		{
			_log.LogWarning("{Method} Did not find a foreground window", nameof(GetForegroundWindowHandle));
			return null;
		}

		_log.LogDebug("{Method} Got foreground window with handle '{Handle}'", nameof(GetForegroundWindowHandle), windowHandle);
		return (nint)windowHandle;
	}

	/// <inheritdoc/>
	public string? GetWindowClass(nint windowHandle)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		var buffer = new Span<char>(new char[256]);
		var length = PI.GetClassName((HWND)windowHandle, buffer);
		return buffer[..length].ToString();
	}

	/// <inheritdoc/>
	public ICollection<Win32Window> GetWindowList()
	{
		_log.LogTrace("{MethodName}", nameof(GetWindowList));

		// Create an empty list of window handles we'll pass to ReportWindow, for it to add the window that was found.
		var windowHandles = new List<HWND>();

		// Pin the list so it doesn't get moved by GC.
		var listHandle = GCHandle.Alloc(windowHandles);
		var listHandlePtr = GCHandle.ToIntPtr(listHandle);

		// Now ask EnumWindows to fill up the list.
		PI.EnumWindows(ReportWindow, listHandlePtr);

		// Turn the list of handles into a list of objects that contain info about the windows.
		return windowHandles
			.Select(w => GetWin32Window(w))
			.Where(w => w != null)
			.Select(w => w!)
			.ToList();
	}

	/// <inheritdoc/>
	public Rectangle GetWindowRect(nint windowHandle)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		if (!PI.GetWindowRect((HWND)windowHandle, out var rect))
		{
			throw new InvalidOperationException($"Could not get window rect for window with handle '{windowHandle}'.", new Win32Exception());
		}

		var res = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
		_log.LogDebug("{Method} Got window rect '{Rectangle}' for window with handle '{WindowHandle}'", nameof(GetWindowRect), res, windowHandle);

		return res;
	}

	/// <inheritdoc/>
	public string? GetWindowTitle(nint windowHandle)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		var buffer = new Span<char>(new char[256]);
		var length = PI.GetWindowText((HWND)windowHandle, buffer);
		return buffer[..length].ToString();
	}

	/// <inheritdoc/>
	public bool IsValidWindow(nint windowHandle)
	{
		// When the window handle is zero, we don't need to ask the OS, as that is invalid by default.
		if (windowHandle == 0)
		{
			return false;
		}

		return PI.IsWindow((HWND)windowHandle);
	}

	/// <inheritdoc/>
	public void MoveWindow(nint windowHandle, Rectangle rectangle)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		_log.LogTrace("{MethodName}({WindowHandle}, {Rectangle})", nameof(MoveWindow), windowHandle, rectangle);

		if (!PI.MoveWindow((HWND)windowHandle, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, true))
		{
			throw new InvalidOperationException($"Could not set size and position to '{rectangle}' of window with handle '{windowHandle}'.", new Win32Exception());
		}
	}

	/// <inheritdoc/>
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
			throw new InvalidOperationException($"Could not set 'Always On Top' to '{isAlwaysOnTop}' of window with handle '{windowHandle}'.", new Win32Exception());
		}
	}

	/// <inheritdoc/>
	public unsafe void SetForegroundWindow(nint windowHandle)
	{
		// Usually, we can just call <see cref="SetForegroundWindow"/>, and we're done.<br/>
		// However, to prevent abuse of this feature, Microsoft implemented a couple rules around setting foreground windows:
		// https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setforegroundwindow#remarks.
		//
		// Usually, of the second set of criteria (of which we need to hit at least 1), we'd hit this one:
		// - The calling process received the last input event.
		//
		// That's because when a hotkey is pressed, WTQ receives an input event, and hence received "the last input event".
		//
		// But in some other cases, like sending a command to WTQ without pressing a hotkey, none of these criteria might be hit,
		// and calling <see cref="SetForegroundWindow"/> doesn't do anything.
		//
		// A trick that apparently is also used by the Chromium team, is to send a synthetic input event, which would
		// make our process the one with "the last input event".
		// https://stackoverflow.com/a/13881647.
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		_log.LogTrace("{MethodName}({WindowHandle})", nameof(SetForegroundWindow), windowHandle);

		var hwnd = (HWND)windowHandle;

		// Attempt the regular method first, simpler and faster.
		PI.SetForegroundWindow(hwnd);
		PI.SendMessage(hwnd, PI.WM_PAINT, 0, 0);

		// Wait for the target window to process a null event, which means the previously sent "become foreground"-event has also been processed.
		// Do this with a timeout, so we don't hang if the target window hangs.
		// https://devblogs.microsoft.com/oldnewthing/20161118-00/?p=94745
		PI.SendMessageTimeout(hwnd, WM_NULL, 0, 0, SEND_MESSAGE_TIMEOUT_FLAGS.SMTO_NORMAL, uTimeout: 100);

		// If the requested window has become the foreground window, we're done.
		if (PI.GetForegroundWindow() == hwnd)
		{
			return;
		}

		_log.LogWarning("{MethodName} failed, attempting synthetic input event workaround", nameof(SetForegroundWindow));

		// Simulate Alt key press and release.
		PI.keybd_event((byte)VIRTUAL_KEY.VK_MENU, 0, 0, UIntPtr.Zero);
		PI.keybd_event((byte)VIRTUAL_KEY.VK_MENU, 0, KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP, UIntPtr.Zero);

		Thread.Sleep(20); // Give Windows a moment to process input events.

		// We should now be the app with the most recent input event, so we should be allowed to set the foreground window.
		PI.SetForegroundWindow(hwnd);
		PI.SendMessage(hwnd, PI.WM_PAINT, 0, 0);

		// Wait for the above event to be processed again.
		PI.SendMessageTimeout(hwnd, WM_NULL, 0, 0, SEND_MESSAGE_TIMEOUT_FLAGS.SMTO_NORMAL, uTimeout: 100);

		if (PI.GetForegroundWindow() == hwnd)
		{
			_log.LogDebug("Synthetic input event workaround successful");
		}
		else
		{
			_log.LogWarning("Synthetic input event workaround failed, window may not have focus");
		}
	}

	/// <inheritdoc/>
	public void SetWindowTitle(nint windowHandle, string title)
	{
		Guard.Against.OutOfRange(windowHandle, nameof(windowHandle), 1, nint.MaxValue);

		PI.SetWindowText((HWND)windowHandle, title);
	}

	/// <inheritdoc/>
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
			throw new InvalidOperationException($"Could not set transparency to '{transparency}' of window with handle '{windowHandle}'.", new Win32Exception());
		}
	}

	/// <summary>
	/// This method is called for each window that is encountered when calling <see cref="PI.EnumWindows(WNDENUMPROC, LPARAM)"/>.
	/// </summary>
	private static unsafe BOOL ReportWindow(HWND windowHandle, LPARAM lParam)
	{
		// Fetch the list of handles that was passed in when calling EnumWindows.
		var listHandle = GCHandle.FromIntPtr(lParam);

		if (listHandle.Target is not List<HWND> windowHandles)
		{
			throw new InvalidOperationException($"Expected list handle to be of type '{typeof(List<HWND>).FullName}', but was '{listHandle.Target?.GetType()?.FullName}'.");
		}

		windowHandles.Add(windowHandle);

		return true; // Returning "true" means that we'll continue onto the next window. Which we want, cause we want to see all windows.
	}

	/// <summary>
	/// Takes a window handle, and returns a decorated <see cref="Win32Window"/>.
	/// </summary>
	private unsafe Win32Window? GetWin32Window(HWND windowHandle)
	{
		var style = PI.GetWindowLong(windowHandle, WINDOW_LONG_PTR_INDEX.GWL_STYLE);

		uint processId = 0;
		var threadId = PI.GetWindowThreadProcessId(windowHandle, &processId);

		// If no thread- or process id could be obtained, skip the window. This may be due to the process having exited in the meantime?
		if (threadId == 0 || processId == 0)
		{
			return null;
		}

		// If we can't get the window's dimensions, skip the window. Perhaps also due to the window just having closed, or it being some hidden thing or whatever.
		if (!PI.GetWindowRect(windowHandle, out var rt))
		{
			return null;
		}

		// Get information about the owning process.
		var ownerProcess = Process.GetProcessById((int)processId);

		// Figure out whether this is the process's main window.
		var isMainWindow = ownerProcess.MainWindowHandle.Equals(windowHandle);

		// Fetch the window class.
		var windowClass = GetWindowClass(windowHandle);

		// Fetch the window class.
		var windowTitle = GetWindowTitle(windowHandle);

		// Construct return object.
		var window = new Win32Window(() => ownerProcess.HasExited)
		{
			IsMainWindow = isMainWindow,
			MainWindowHandle = ownerProcess.MainWindowHandle,
			ProcessId = processId,
			ProcessName = ownerProcess.ProcessName,
			Rect = new(rt.left, rt.top, rt.right - rt.left, rt.bottom - rt.top),
			Style = style,
			ThreadId = threadId,
			WindowCaption = windowTitle,
			WindowClass = windowClass,
			WindowHandle = windowHandle,
		};

		return window;
	}
}