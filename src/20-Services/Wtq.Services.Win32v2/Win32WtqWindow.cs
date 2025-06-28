using System.Text.RegularExpressions;
using Wtq.Services.Win32v2.Native;

namespace Wtq.Services.Win32v2;

public sealed class Win32WtqWindow : WtqWindow
{
	private readonly ILogger _log;

	private readonly IWin32 _win32;
	private readonly Win32Window _window;

	public Win32WtqWindow(IWin32 win32, Win32Window window)
	{
		_win32 = Guard.Against.Null(win32);
		_window = Guard.Against.Null(window);

		_log = Log.For($"{nameof(Win32WtqWindow)}|{window}");
	}

	[CanBeMatchedOn]
	public override string Id =>
		_window.WindowHandle.ToString(CultureInfo.InvariantCulture);

	public bool IsMainWindow =>
		_window.IsMainWindow;

	public override bool IsValid =>
		!_window.HasExited && // Check whether the process that owns the window is still running.
		_win32.IsValidWindow(_window.WindowHandle); // Check whether the window itself is still valid (could be closed while the owning process is still running).

	[CanBeMatchedOn]
	public override string? Name =>
		_window.ProcessName;

	[CanBeMatchedOn]
	public uint ProcessId =>
		_window.ProcessId;

	[CanBeMatchedOn]
	public string? ProcessName =>
		_window.ProcessName;

	public Rectangle Rect =>
		_window.Rect;

	[CanBeMatchedOn]
	public uint ThreadId =>
		_window.ThreadId;

	[CanBeMatchedOn]
	public string? WindowClass =>
		_window.WindowClass;

	[CanBeMatchedOn]
	public nint WindowHandle =>
		_window.WindowHandle;

	[CanBeMatchedOn]
	public override string? WindowTitle =>
		_window.WindowCaption;

	public override bool Matches(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		// Process name
		var expectedProcName = opts.ProcessName?.EmptyOrWhiteSpaceToNull() ?? System.IO.Path.GetFileNameWithoutExtension(opts.FileName)!; // If no process name was specified, use the filename instead (but without .exe or such).
		if (!expectedProcName.Equals(_window.ProcessName, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		// Window class
		if (!string.IsNullOrWhiteSpace(opts.WindowClass) && !string.IsNullOrWhiteSpace(_window.WindowClass) && !Regex.IsMatch(opts.WindowClass, _window.WindowClass, RegexOptions.IgnoreCase))
		{
			return false;
		}

		// Window title
		if (!string.IsNullOrWhiteSpace(opts.WindowTitle) && !string.IsNullOrWhiteSpace(_window.WindowCaption) && !Regex.IsMatch(opts.WindowTitle, _window.WindowCaption, RegexOptions.IgnoreCase))
		{
			return false;
		}

		return true;
	}

	public override Task BringToForegroundAsync()
	{
		_log.LogDebug("{MethodName}", nameof(BringToForegroundAsync));

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

		// Update window object.
		_window.Rect = rect;
	}

	public override async Task SetSizeAsync(Size size)
	{
		if (size.Width < 200 || size.Height < 200)
		{
			throw new InvalidOperationException($"Attempted to set window size to something under 200x200, which can mess up windows.");
		}

		// Get current window rect.
		var rect = await GetWindowRectAsync().NoCtx();

		// Set new size.
		rect.Size = size;

		// Update window rect.
		_win32.MoveWindow(_window.WindowHandle, rect);

		// Update window object.
		_window.Rect = rect;
	}

	public override Task SetTaskbarIconVisibleAsync(bool isVisible)
	{
		// Get handle to the main window
		var handle = _window.WindowHandle;

		_log.LogDebug("Setting taskbar icon visibility for process with main window handle '{Handle}'", handle);

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