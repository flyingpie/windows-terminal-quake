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
		!_window.Process.HasExited && // Check whether the process that owns the window is still running.
		_win32.IsValidWindow(_window.WindowHandle); // Check whether the window itself is still valid (could be closed while the owning process is still running).

	[CanBeMatchedOn]
	public override string? Name =>
		_window.Process.ProcessName;

	[CanBeMatchedOn]
	public uint ProcessId =>
		_window.ProcessId;

	[CanBeMatchedOn]
	public string? ProcessName =>
		_window.Process.ProcessName;

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

		var expectedProcName = opts.ProcessName;
		if (string.IsNullOrWhiteSpace(expectedProcName))
		{
			expectedProcName = System.IO.Path.GetFileNameWithoutExtension(opts.FileName);
		}

		// TODO: Add regex support to matching.

		// Window class
		if (!string.IsNullOrWhiteSpace(opts.WindowClass) && !opts.WindowClass.Equals(_window.WindowClass, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		// Window title
		if (!string.IsNullOrWhiteSpace(opts.WindowTitle))
		{
			var r = new Regex(opts.WindowTitle, RegexOptions.IgnoreCase);
			if (!r.IsMatch(_window.WindowCaption ?? string.Empty))
			{
				return false;
			}
//			!opts.WindowTitle.Equals(_window.WindowCaption, StringComparison.OrdinalIgnoreCase)

//			return false;
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