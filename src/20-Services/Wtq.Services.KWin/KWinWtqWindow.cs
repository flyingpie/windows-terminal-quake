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

		// Match by file name.
		if (!MatchesFileName(opts))
		{
			return false;
		}

		// TODO
		// Match by resource class (often reverse DNS notation).
		// if (!string.IsNullOrWhiteSpace(opts.ResourceClass) && opts.ResourceClass.Equals(_window.ResourceClass, StringComparison.OrdinalIgnoreCase))
		// {
		//   return true;
		// }

		// TODO
		// Match by resource name (close to process name).
		// if (!string.IsNullOrWhiteSpace(opts.ResourceName) && opts.ResourceName.Equals(_window.ResourceName, StringComparison.OrdinalIgnoreCase))
		// {
		//   return true;
		// }

		// Match by window title.
		if (!string.IsNullOrWhiteSpace(opts.WindowTitle) && !Regex.IsMatch(_window.Caption ?? string.Empty, opts.WindowTitle, RegexOptions.IgnoreCase))
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

	private bool MatchesFileName(WtqAppOptions opts)
	{
		// If we don't have a filename in the options, consider that "matches anything".
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
}