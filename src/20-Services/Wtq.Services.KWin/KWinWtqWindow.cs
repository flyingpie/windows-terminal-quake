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

	/// <summary>
	/// TODO: Add proper window activity checking.
	/// - Does the window still exist?
	/// - Is the window still valid/movable/whatever?
	/// - etc.
	/// </summary>
	public override bool IsValid =>
		_isValid;

	public override string? Name =>
		_window?.ResourceName;

	public string? DesktopFileName =>
		_window?.DesktopFileName;

	public override string? WindowTitle =>
		_window?.Caption;

	public string? ResourceClass =>
		_window?.ResourceClass;

	public string? ResourceName =>
		_window?.ResourceName;

	public string? FrameGeometry =>
		_window?.FrameGeometry?.ToString();

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

		var searchTerm = opts.ProcessName;
		if (string.IsNullOrWhiteSpace(searchTerm) && !string.IsNullOrWhiteSpace(opts.FileName))
		{
			searchTerm = Path.GetFileNameWithoutExtension(opts.FileName);
		}

		if (string.IsNullOrWhiteSpace(searchTerm))
		{
			searchTerm = null;
		}

		// Match by resource class (often reverse DNS notation).
		if (searchTerm != null && searchTerm.Equals(_window.ResourceClass, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		// Match by resource name (close to process name).
		if (searchTerm != null && searchTerm.Equals(_window.ResourceName, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		// Match by window title.
		if (!string.IsNullOrWhiteSpace(opts.WindowTitle) && opts.WindowTitle.Equals(_window.Caption, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		return false;
	}

	public override async Task MoveToAsync(Point location)
	{
		await _kwinClient.MoveWindowAsync(_window, location, CancellationToken.None).NoCtx();
	}

	public override async Task ResizeAsync(Size size)
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
}