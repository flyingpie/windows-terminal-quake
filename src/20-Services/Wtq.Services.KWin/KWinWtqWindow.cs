using Wtq.Configuration;
using Wtq.Services.KWin.Dto;

namespace Wtq.Services.KWin;

public class KWinWtqWindow(
	IKWinClient kwinClient,
	KWinWindow window)
	: WtqWindow
{
	private readonly IKWinClient _kwinClient = Guard.Against.Null(kwinClient);
	private readonly KWinWindow _window = Guard.Against.Null(window);

	private bool _isValid = true;

	public override string Id => _window.InternalId ?? "<unknown>";

	#region Generic Stuff

	[CanBeMatchedOn]
	public string? FileName =>
		_window?.DesktopFileName;

	#endregion

	[CanBeMatchedOn]
	public string? DesktopFileName =>
		_window?.DesktopFileName;

	public Rectangle? FrameGeometry =>
		_window?.FrameGeometry?.ToRect();

	/// <summary>
	/// TODO: Add proper window activity checking.
	/// - Does the window still exist?
	/// - Is the window still valid/movable/whatever?
	/// - etc.
	/// </summary>
	public override bool IsValid =>
		_isValid;

	[CanBeMatchedOn]
	public override string? Name =>
		_window?.ResourceName;

	[CanBeMatchedOn]
	public string? ResourceClass =>
		_window?.ResourceClass;

	[CanBeMatchedOn]
	public string? ResourceName =>
		_window?.ResourceName;

	[CanBeMatchedOn]
	public override string? WindowTitle =>
		_window?.Caption;

	public override async Task BringToForegroundAsync()
	{
		await _kwinClient.BringToForegroundAsync(_window, CancellationToken.None).NoCtx();
	}

	public override async Task<Rectangle> GetWindowRectAsync()
	{
		var w = await _kwinClient.GetWindowAsync(_window, CancellationToken.None).NoCtx();

		// TODO: Handle null.
		return w.FrameGeometry?.ToRect() ?? Rectangle.Empty;
	}

	public override bool Matches(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		var hasMatchers =
			!string.IsNullOrWhiteSpace(opts.WindowTitle) ||
			!string.IsNullOrWhiteSpace(opts.ProcessName);

		// If no other criteria are set, match on the "FileName" setting.
		if (!hasMatchers)
		{
			// If no "FileName" criterion has been set either, don't match anything.
			return !string.IsNullOrWhiteSpace(opts.FileName) && MatchesFileName(opts);
		}

		// Process name
		if (!MatchesProcessName(opts.ProcessName))
		{
			return false;
		}

		// Window title
		if (!MatchesWindowTitle(opts.WindowTitle))
		{
			return false;
		}

		return true;
	}

	public override async Task SetLocationAsync(Point location)
	{
		await _kwinClient.MoveWindowAsync(_window, location, CancellationToken.None).NoCtx();
	}

	public override async Task SetSizeAsync(Size size)
	{
		await _kwinClient.ResizeWindowAsync(_window, size, CancellationToken.None).NoCtx();
	}

	public override async Task SetAlwaysOnTopAsync(bool isAlwaysOnTop)
	{
		await _kwinClient.SetWindowAlwaysOnTopAsync(_window, isAlwaysOnTop, CancellationToken.None).NoCtx();
	}

	public override async Task SetTaskbarIconVisibleAsync(bool isVisible)
	{
		await _kwinClient.SetTaskbarIconVisibleAsync(_window, isVisible, CancellationToken.None).NoCtx();
	}

	public override async Task SetTransparencyAsync(int transparency)
	{
		await _kwinClient.SetWindowOpacityAsync(_window, transparency * .01f, CancellationToken.None).NoCtx();
	}

	public override Task SetWindowTitleAsync(string title)
	{
		// TODO
		return Task.CompletedTask;
	}

	public override async Task UpdateAsync()
	{
		var w = await _kwinClient.GetWindowAsync(_window, CancellationToken.None).NoCtx();

		_isValid = !string.IsNullOrWhiteSpace(w?.InternalId);
	}

	/// <summary>
	/// The filename is primarily used to start an app.<br/>
	/// Historically, it was assumed the filename was enough to match a window (and thus also used for that),
	/// but this turns out to not be the case.<br/>
	/// <br/>
	/// So now, we can configure additional matchers (like resource name or window title).<br/>
	/// If no matchers are specified, we try to match the filename against multiple window properties.<br/>
	/// </summary>
	private bool MatchesFileName(WtqAppOptions opts)
	{
		// If we don't have a filename in the options, consider that "matches anything".
		// It is assumed another matcher will be used, as the app options will only be valid with at least 1 matcher.
		if (string.IsNullOrWhiteSpace(opts.FileName))
		{
			return true;
		}

		// Check the actual filename first.
		if (opts.FileName.Equals(_window.DesktopFileName, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		// Match by resource class (often reverse DNS notation).
		if (opts.FileName.Equals(_window.ResourceClass, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		// Match by resource name (close to process name).
		if (opts.FileName.Equals(_window.ResourceName, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// Process name is really a Windows thing, but in an effort to keep the configuration as cross-platform as possible,
	/// we interpret various windows properties as "process name".<br/>
	/// They're functionally very similar anyway.
	/// </summary>
	private bool MatchesProcessName(string processName)
	{
		// If we don't have a process name in the options, consider that "matches anything".
		// It is assumed another matcher will be used, as the app options will only be valid with at least 1 matcher.
		if (string.IsNullOrWhiteSpace(processName))
		{
			return true;
		}

		// Match by file name that started the app.
		if (Regex.IsMatch(_window.DesktopFileName ?? string.Empty, processName, RegexOptions.IgnoreCase))
		{
			return true;
		}

		// Match by resource class (often reverse DNS notation).
		if (Regex.IsMatch(_window.ResourceClass ?? string.Empty, processName, RegexOptions.IgnoreCase))
		{
			return true;
		}

		// Match by resource name (close to process name).
		if (Regex.IsMatch(_window.ResourceName ?? string.Empty, processName, RegexOptions.IgnoreCase))
		{
			return true;
		}

		return false;
	}

	private bool MatchesWindowTitle(string windowTitle)
	{
		// If we don't have a window title in the options, consider that "matches anything".
		// It is assumed another matcher will be used, as the app options will only be valid with at least 1 matcher.
		if (string.IsNullOrWhiteSpace(windowTitle))
		{
			return true;
		}

		// Match by file name that started the app.
		if (Regex.IsMatch(_window.Caption ?? string.Empty, windowTitle, RegexOptions.IgnoreCase))
		{
			return true;
		}

		return false;
	}
}