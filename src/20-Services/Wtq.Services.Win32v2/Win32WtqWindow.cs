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
		if (!string.IsNullOrWhiteSpace(opts.ProcessName))
		{
			if (!Regex.IsMatch(_window.ProcessName ?? string.Empty, opts.ProcessName, RegexOptions.IgnoreCase))
			{
				return false;
			}
		}

		// If no process name was specified, use the filename instead (but without .exe or such).
		else if (!string.IsNullOrWhiteSpace(opts.FileName))
		{
			var fileName = opts.FileName.GetFileNameWithoutExtension();
			if (!fileName.Equals(_window.ProcessName, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
		}

		// Window class
		if (!string.IsNullOrWhiteSpace(opts.WindowClass) && !Regex.IsMatch(_window.WindowClass ?? string.Empty, opts.WindowClass, RegexOptions.IgnoreCase))
		{
			return false;
		}

		// Window title
		if (!string.IsNullOrWhiteSpace(opts.WindowTitle) && !Regex.IsMatch(_window.WindowCaption ?? string.Empty, opts.WindowTitle, RegexOptions.IgnoreCase))
		{
			return false;
		}

		return true;
	}

	public override Task BringToForegroundAsync()
	{
		_log.LogDebug("{MethodName}", nameof(BringToForegroundAsync));

		try
		{
			_win32.SetForegroundWindow(_window.WindowHandle);
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Could set foreground window '{Window}': {Message}", this, ex.Message);
		}

		return Task.CompletedTask;
	}

	public override Task<Rectangle> GetWindowRectAsync()
	{
		var rect = _win32.GetWindowRect(_window.WindowHandle);

		return Task.FromResult(rect);
	}

	public override Task SetAlwaysOnTopAsync(bool isAlwaysOnTop)
	{
		try
		{
			_win32.SetAlwaysOnTop(_window.WindowHandle, isAlwaysOnTop);
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Could set 'always on top' window '{Window}' to '{IsAlwaysOnTop}': {Message}", this, isAlwaysOnTop, ex.Message);
		}

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
		try
		{
			Shell32.SetTaskbarIconVisible(_window.WindowHandle, isVisible);
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Could not set visibility for window '{Window}' to '{IsVisible}': {Message}", this, isVisible, ex.Message);
		}

		return Task.CompletedTask;
	}

	public override Task SetTransparencyAsync(int transparency)
	{
		try
		{
			_win32.SetWindowTransparency(_window.WindowHandle, transparency);
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Could not set transparency for window '{Window}' to '{Transparency}': {Message}", this, transparency, ex.Message);
		}

		return Task.CompletedTask;
	}

	public override Task SetWindowTitleAsync(string title)
	{
		try
		{
			_win32.SetWindowTitle(_window.WindowHandle, title);
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Could not set title for window '{Window}' to '{Title}': {Message}", this, title, ex.Message);
		}

		return Task.CompletedTask;
	}

	public override string ToString() => $"ProcessName:{ProcessName} IsMainWindow:{IsMainWindow} WindowClass:{WindowClass} Title:'{WindowTitle}' ProcessId:{ProcessId} WindowHandle:{Id} Location:{Rect.Location.ToShortString()} Size:{Rect.Size.ToShortString()}";

	public override Task UpdateAsync() => Task.CompletedTask;
}