using Wtq.Services.Win32.Native;

namespace Wtq.Services.Win32;

public sealed class Win32WtqWindow(
	Win32Window window)
	: WtqWindow
{
	private static readonly ILogger _log = Log.For<Win32WtqWindow>();

	private readonly Win32Window _window = Guard.Against.Null(window);

	public override string Id =>
		_window.WindowHandle.ToString(CultureInfo.InvariantCulture);

	public override bool IsValid =>
		!_window.Process.HasExited;

	public override string? Name =>
		_window.Process.ProcessName;

	public override string? WindowTitle =>
		_window.WindowCaption;

	public string? ProcessName =>
		_window.Process.ProcessName;

	public uint ProcessId =>
		_window.ProcessId;

	public uint ThreadId =>
		_window.ThreadId;

	public nint WindowHandle =>
		_window.WindowHandle;

	public Rectangle Rect =>
		_window.Rect;

	public string WindowClass =>
		_window.WindowClass;

	public override Task BringToForegroundAsync()
	{
		User32.SetForegroundWindow(_window.WindowHandle);
		User32.ForcePaint(_window.WindowHandle);

		return Task.CompletedTask;
	}

	public override Task<Rectangle> GetWindowRectAsync()
	{
		var bounds = default(Bounds);

		User32.GetWindowRect(_window.WindowHandle, ref bounds);

		return Task.FromResult(bounds.ToRectangle());
	}

	public override bool Matches(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		var expectedProcName = opts.ProcessName;
		if (string.IsNullOrWhiteSpace(expectedProcName))
		{
			expectedProcName = System.IO.Path.GetFileNameWithoutExtension(opts.FileName);
		}

		// Window class
		if (!string.IsNullOrWhiteSpace(opts.WindowClass) && !opts.WindowClass.Equals(_window.WindowClass, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		// Window title
		if (!string.IsNullOrWhiteSpace(opts.WindowTitle) && !opts.WindowTitle.Equals(_window.WindowCaption, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		// Process name
		if (!expectedProcName.Equals(_window.Process.ProcessName, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		return true;
	}

	public override async Task MoveToAsync(Point location)
	{
		var r = await GetWindowRectAsync().NoCtx();

		User32.MoveWindow(
			hWnd: _window.WindowHandle,
			x: location.X,
			y: location.Y,
			nWidth: r.Width,
			nHeight: r.Height,
			bRepaint: true);
	}

	public override async Task ResizeAsync(Size size)
	{
		var r = await GetWindowRectAsync().NoCtx();

		User32.MoveWindow(
			hWnd: _window.WindowHandle,
			x: r.X,
			y: r.Y,
			nWidth: size.Width,
			nHeight: size.Height,
			bRepaint: true);
	}

	public override Task SetAlwaysOnTopAsync(bool isAlwaysOnTop)
	{
		if (_window.WindowHandle == IntPtr.Zero)
		{
			throw new WtqException("Process handle zero");
		}

		var isSet = User32.SetWindowPos(
			_window.WindowHandle,
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
		var handle = _window.WindowHandle;

		_log.LogInformation("Setting taskbar icon visibility for process with main window handle '{Handle}'", handle);

		Shell32.SetTaskbarIconVisible(handle, isVisible);

		return Task.CompletedTask;
	}

	public override Task SetTransparencyAsync(int transparency)
	{
		if (transparency >= 100)
		{
			return Task.CompletedTask;
		}

		if (_window.WindowHandle == IntPtr.Zero)
		{
			throw new WtqException("Process handle zero");
		}

		// Get original window properties
		var props = User32.GetWindowLong(_window.WindowHandle, User32.GWLEXSTYLE);

		// Add "WS_EX_LAYERED"-flag (required for transparency).
		User32.SetWindowLong(_window.WindowHandle, User32.GWLEXSTYLE, props | User32.WSEXLAYERED);

		// Set transparency
		var isSet = User32.SetLayeredWindowAttributes(
			_window.WindowHandle,
			0,
			(byte)Math.Ceiling(255f / 100f * transparency),
			User32.LWAALPHA);

		if (!isSet)
		{
			throw new WtqException("Could not set window opacity");
		}

		return Task.CompletedTask;
	}

	public override Task SetWindowTitleAsync(string title)
	{
		User32.SetWindowText(_window.WindowHandle, title);

		return Task.CompletedTask;
	}

	public override Task UpdateAsync() => Task.CompletedTask;
}