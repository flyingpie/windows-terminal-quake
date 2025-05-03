using Wtq.Services.Win32.Native;

namespace Wtq.Services.Win32;

public sealed class Win32WtqWindow(
	IWin32 win32,
	Win32Window window)
	: WtqWindow
{
	private static readonly ILogger _log = Log.For<Win32WtqWindow>();

	private readonly IWin32 _win32 = Guard.Against.Null(win32);
	private readonly Win32Window _window = Guard.Against.Null(window);

	public override string Id =>
		_window.WindowHandle.ToString(CultureInfo.InvariantCulture);

	public bool IsMainWindow =>
		_window.IsMainWindow;

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

	public override Task BringToForegroundAsync()
	{
		_win32.SetForegroundWindow(_window.WindowHandle);

		return Task.CompletedTask;
	}

	public override Task<Rectangle> GetWindowRectAsync()
	{
		var rect = _win32.GetWindowRect(_window.WindowHandle);

		return Task.FromResult(rect);
	}

	public override Task SetAlwaysOnTopAsync(bool isAlwaysOnTop)
	{
		_win32.SetAlwaysOnTop(_window.WindowHandle, isAlwaysOnTop);

		return Task.CompletedTask;
	}

	public override async Task SetLocationAsync(Point location)
	{
		// Get current window rect.
		var rect = await GetWindowRectAsync().NoCtx();

		// Set new location.
		rect.Location = location;

		// Update window rect.
		_win32.MoveWindow(_window.WindowHandle, rect);
	}

	public override async Task SetSizeAsync(Size size)
	{
		// Get current window rect.
		var rect = await GetWindowRectAsync().NoCtx();

		// Set new size.
		rect.Size = size;

		// Update window rect.
		_win32.MoveWindow(_window.WindowHandle, rect);
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
		_win32.SetWindowTransparency(_window.WindowHandle, transparency);

		return Task.CompletedTask;
	}

	public override Task SetWindowTitleAsync(string title)
	{
		_win32.SetWindowTitle(_window.WindowHandle, title);

		return Task.CompletedTask;
	}

	public override string ToString() => $"WindowHandle:{Id} ProcessId:{ProcessId} ProcessName:{ProcessName} Title:{WindowTitle} WindowClass:{WindowClass} Rect:{Rect} IsMainWindow:{IsMainWindow}";

	public override Task UpdateAsync() => Task.CompletedTask;
}