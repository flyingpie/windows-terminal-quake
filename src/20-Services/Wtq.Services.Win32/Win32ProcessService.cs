using Wtq.Data;
using Wtq.Exceptions;
using Wtq.Services.Win32.Native;
using Wtq.Utils;

namespace Wtq.Services.Win32;

public sealed class Win32ProcessService : IWtqProcessService
{
	private readonly ILogger _log = Log.For<Win32ProcessService>();
	private readonly object _procLock = new();
	private readonly TimeSpan _lookupInterval = TimeSpan.FromSeconds(2);

	private DateTimeOffset _nextLookup = DateTimeOffset.MinValue;
	private IEnumerable<Process> _processes = [];

	/// <summary>
	/// Bring the process' main window to the foreground.
	/// </summary>
	public void BringToForeground(Process process)
	{
		Guard.Against.Null(process);

		// TODO: Only do this in cases where we want the app to disappear? Toggling window state causes flickering.
		// SetWindowState(WindowShowStyle.Restore);
		User32.SetForegroundWindow(process.MainWindowHandle);
		User32.ForcePaint(process.MainWindowHandle);

		// User32.ShowWindow(process.MainWindowHandle, WindowShowStyle.Show);
		// User32.SendMessage(process.MainWindowHandle, )
	}

	public Process? GetForegroundProcess()
	{
		try
		{
			var fg = GetForegroundProcessId();
			if (fg > 0)
			{
				return Process.GetProcessById((int)fg);
			}
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error looking up foreground process: {Message}", ex.Message);
		}

		return null;
	}

	public uint GetForegroundProcessId()
	{
		var hwnd = User32.GetForegroundWindow();
		User32.GetWindowThreadProcessId(hwnd, out uint pid);

		return pid;
	}

	public IEnumerable<Process> GetProcesses()
	{
		lock (_procLock)
		{
			if (_nextLookup < DateTimeOffset.UtcNow)
			{
				_log.LogDebug("Looking up list of processes");
				_nextLookup = DateTimeOffset.UtcNow.Add(_lookupInterval);
				_processes = Process.GetProcesses();
			}
		}

		return _processes;
	}

	public WtqRect GetWindowRect(Process process)
	{
		Guard.Against.Null(process);

		var bounds = default(Bounds);

		User32.GetWindowRect(process.MainWindowHandle, ref bounds);

		return bounds.ToWtqBounds().ToWtqRect();
	}

	/// <summary>
	/// Sets the position and size of the process' main window.
	/// </summary>
	public void MoveWindow(Process process, WtqRect rect, bool repaint = true)
	{
		Guard.Against.Null(process);

		User32.MoveWindow(
			hWnd: process.MainWindowHandle,
			x: rect.X,
			y: rect.Y,
			nWidth: rect.Width,
			nHeight: rect.Height,
			bRepaint: repaint);
	}

	/// <summary>
	/// Make sure the window is always the top-most one.
	/// </summary>
	public void SetAlwaysOnTop(Process process)
	{
		Guard.Against.Null(process);

		if (process.MainWindowHandle == IntPtr.Zero)
		{
			throw new WtqException("Process handle zero");
		}

		var isSet = User32.SetWindowPos(process.MainWindowHandle, User32.HWNDTOPMOST, 0, 0, 0, 0, User32.TOPMOSTFLAGS);
		if (!isSet)
		{
			throw new WtqException("Could not set window top most");
		}
	}

	/// <summary>
	/// Hides- or shows the taskbar icon of the specified process.
	/// </summary>
	public void SetTaskbarIconVisibility(Process process, bool isVisible)
	{
		Guard.Against.Null(process);

		// Get handle to the main window
		var handle = process.MainWindowHandle;

		_log.LogInformation("Setting taskbar icon visibility for process with main window handle '{Handle}'", handle);

		// Get current window properties
		var props = User32.GetWindowLong(handle, User32.GWLEXSTYLE);

		if (isVisible)
		{
			// Show
			User32.SetWindowLong(handle, User32.GWLEXSTYLE, (props | User32.WSEXTOOLWINDOW) & User32.WSEXAPPWINDOW);
		}
		else
		{
			// Hide
			User32.SetWindowLong(handle, User32.GWLEXSTYLE, (props | User32.WSEXTOOLWINDOW) & ~User32.WSEXAPPWINDOW);
		}
	}

	/// <summary>
	/// Makes the entire window of the specified process transparent.
	/// </summary>
	public void SetTransparency(Process process, int transparency)
	{
		Guard.Against.Null(process);

		if (process.MainWindowHandle == IntPtr.Zero)
		{
			throw new WtqException("Process handle zero");
		}

		// Get original window properties
		var props = User32.GetWindowLong(process.MainWindowHandle, User32.GWLEXSTYLE);

		// Add "WS_EX_LAYERED"-flag (required for transparency).
		User32.SetWindowLong(process.MainWindowHandle, User32.GWLEXSTYLE, props | User32.WSEXLAYERED);

		// Set transparency
		var isSet = User32.SetLayeredWindowAttributes(process.MainWindowHandle, 0, (byte)Math.Ceiling(255f / 100f * transparency), User32.LWAALPHA);
		if (!isSet)
		{
			throw new WtqException("Could not set window opacity");
		}
	}
}