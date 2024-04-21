using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Wtq.Data;
using Wtq.Exceptions;
using Wtq.Services.Win32.Native;
using Wtq.Utils;

namespace Wtq.Services.Win32;

public sealed class Win32ProcessService : IWtqProcessService
{
	/// <summary>
	/// Bring the process' main window to the foreground.
	/// </summary>
	public void BringToForeground(Process process)
	{
		if (process == null) throw new ArgumentNullException(nameof(process));

		// TODO: Only do this in cases where we want the app to disappear? Toggling window state causes flickering.
		//SetWindowState(WindowShowStyle.Restore);
		User32.SetForegroundWindow(process.MainWindowHandle);
		User32.ForcePaint(process.MainWindowHandle);
		//User32.ShowWindow(process.MainWindowHandle, WindowShowStyle.Show);
		//User32.SendMessage(process.MainWindowHandle, )
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
		}

		return null;
	}

	public uint GetForegroundProcessId()
	{
		IntPtr hwnd = User32.GetForegroundWindow();
		uint pid;
		User32.GetWindowThreadProcessId(hwnd, out pid);
		//Process p = Process.GetProcessById((int)pid);
		//p.MainModule.FileName.Dump();

		return pid;
	}

	private readonly object _procLock = new();
	private IEnumerable<Process> _processes = [];
	private DateTimeOffset _nextLookup = DateTimeOffset.MinValue;
	private TimeSpan _lookupInterval = TimeSpan.FromSeconds(2);
	private readonly ILogger _log = Log.For<Win32ProcessService>();

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
		var bounds = new Bounds();

		User32.GetWindowRect(process.MainWindowHandle, ref bounds);

		return bounds.ToWtqBounds().ToWtqRect();
	}

	/// <summary>
	/// Sets the position and size of the process' main window.
	/// </summary>
	public void MoveWindow(Process process, WtqRect rect, bool repaint = true)
	{
		if (process == null) throw new ArgumentNullException(nameof(process));

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
		if (process == null) throw new ArgumentNullException(nameof(process));

		//retry.Execute(() =>
		//{
		if (process.MainWindowHandle == IntPtr.Zero) throw new WtqException("Process handle zero");

		var isSet = User32.SetWindowPos(process.MainWindowHandle, User32.HWNDTOPMOST, 0, 0, 0, 0, User32.TOPMOSTFLAGS);
		if (!isSet) throw new WtqException("Could not set window top most");
		//});
	}

	/// <summary>
	/// Hides- or shows the taskbar icon of the specified process.
	/// </summary>
	public void SetTaskbarIconVisibility(Process process, bool isVisible)
	{
		if (process == null) throw new ArgumentNullException(nameof(process));

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
	/// <param name="transparency">Desired transparency, value between 0 (invisible) to 100 (opaque).</param>
	public void SetTransparency(Process process, int transparency)
	{
		if (process == null) throw new ArgumentNullException(nameof(process));

		//retry.Execute(() =>
		//{
		if (process.MainWindowHandle == IntPtr.Zero) throw new WtqException("Process handle zero");

		// Get original window properties
		var props = User32.GetWindowLong(process.MainWindowHandle, User32.GWLEXSTYLE);

		// Add "WS_EX_LAYERED"-flag (required for transparency).
		User32.SetWindowLong(process.MainWindowHandle, User32.GWLEXSTYLE, props | User32.WSEXLAYERED);

		// Set transparency
		var isSet = User32.SetLayeredWindowAttributes(process.MainWindowHandle, 0, (byte)Math.Ceiling(255f / 100f * transparency), User32.LWAALPHA);
		if (!isSet) throw new WtqException("Could not set window opacity");
		//});
	}

	//public void SetWindowState(Process process, WindowShowStyle state)
	//{
	//	if (process == null) throw new ArgumentNullException(nameof(process));

	//	User32.ShowWindow(process.MainWindowHandle, state);
	//}

	//private static int GetParentProcess(int Id)
	//{
	//	int parentPid = 0;
	//	using ManagementObject mo = new ManagementObject("win32_process.handle='" + Id.ToString() + "'");

	//	mo.Get();
	//	parentPid = Convert.ToInt32(mo["ParentProcessId"]);

	//	return parentPid;
	//}

	//public string? GetProcessCommandLine(Process process)
	//{
	//	try
	//	{
	//		using var searcher = new ManagementObjectSearcher("SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id);
	//		using var objects = searcher.Get();

	//		return objects.Cast<ManagementBaseObject>().SingleOrDefault()?["CommandLine"]?.ToString();
	//	}
	//	catch
	//	{
	//		return null;
	//	}
	//}
}