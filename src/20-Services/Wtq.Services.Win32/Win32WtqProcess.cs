using Wtq.Configuration;
using Wtq.Exceptions;
using Wtq.Services.Win32.Native;
using Wtq.Utils;

namespace Wtq.Services.Win32;

public sealed class Win32WtqProcess : WtqWindow
{
	private static readonly ILogger _log = Log.For<Win32WtqProcess>();

	// TODO: Refresh?
	private readonly Process _process;

	public Win32WtqProcess(Process process)
	{
		_process = Guard.Against.Null(process);
	}

	public override int Id => _process.Id;

	public override bool IsValid => !_process.HasExited;

	public override string? Name => _process.ProcessName;

	public override Rectangle WindowRect
	{
		get
		{
			var bounds = default(Bounds);

			User32.GetWindowRect(_process.MainWindowHandle, ref bounds);

			return bounds.ToRectangle();
		}
	}

	public override Task BringToForegroundAsync()
	{
		User32.SetForegroundWindow(_process.MainWindowHandle);
		User32.ForcePaint(_process.MainWindowHandle);

		return Task.CompletedTask;
	}

	public override bool Matches(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		var expectedProcName = opts.ProcessName;
		if (string.IsNullOrWhiteSpace(expectedProcName))
		{
			expectedProcName = System.IO.Path.GetFileNameWithoutExtension(opts.FileName);
		}

		return expectedProcName.Equals(_process.ProcessName, StringComparison.OrdinalIgnoreCase);
	}

	public override Task MoveToAsync(Rectangle rect, bool repaint = true)
	{
		User32.MoveWindow(
			hWnd: _process.MainWindowHandle,
			x: rect.X,
			y: rect.Y,
			nWidth: rect.Width,
			nHeight: rect.Height,
			bRepaint: repaint);

		return Task.CompletedTask;
	}

	public override Task SetAlwaysOnTopAsync(bool isAlwaysOnTop)
	{
		if (_process.MainWindowHandle == IntPtr.Zero)
		{
			throw new WtqException("Process handle zero");
		}

		var isSet = User32.SetWindowPos(
			_process.MainWindowHandle,
			isAlwaysOnTop ? User32.HWNDTOPMOST : User32.HWNDNOTOPMOST,
			0,
			0,
			0,
			0,
			User32.TOPMOSTFLAGS);

		if (!isSet)
		{
			throw new WtqException("Could not set window top most");
		}

		return Task.CompletedTask;
	}

	public override Task SetTaskbarIconVisibleAsync(bool isVisible)
	{
		// Get handle to the main window
		var handle = _process.MainWindowHandle;

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

		return Task.CompletedTask;
	}

	public override Task SetTransparencyAsync(int transparency)
	{
		if (transparency >= 100)
		{
			return Task.CompletedTask;
		}

		if (_process.MainWindowHandle == IntPtr.Zero)
		{
			throw new WtqException("Process handle zero");
		}

		// Get original window properties
		var props = User32.GetWindowLong(_process.MainWindowHandle, User32.GWLEXSTYLE);

		// Add "WS_EX_LAYERED"-flag (required for transparency).
		User32.SetWindowLong(_process.MainWindowHandle, User32.GWLEXSTYLE, props | User32.WSEXLAYERED);

		// Set transparency
		var isSet = User32.SetLayeredWindowAttributes(
			_process.MainWindowHandle,
			0,
			(byte)Math.Ceiling(255f / 100f * transparency),
			User32.LWAALPHA);

		if (!isSet)
		{
			throw new WtqException("Could not set window opacity");
		}

		return Task.CompletedTask;
	}

	public override Task SetVisibleAsync(bool isVisible)
	{
		return Task.CompletedTask;
	}
}