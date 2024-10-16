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

	private bool? _isAlwaysOnTop;
	private bool? _isTaskbarIconVisible;
	private int? _transparency;
	private bool? _isVisible;
	private Rectangle _rect = new(0, 0, 1024, 768); // TODO: Get actual window size.

	public override string Id => _window.ResourceClass; // TODO: Use "InternalId"

	public override bool IsValid { get; } = true;

	public override string? Name => _window?.ResourceClass;

	public override async Task BringToForegroundAsync()
	{
		await _kwinClient.BringToForegroundAsync(_window, CancellationToken.None).NoCtx();
	}

	public override async Task<Rectangle> GetWindowRectAsync()
	{
		var w = await _kwinClient.GetWindowAsync(_window).NoCtx();

		return w.FrameGeometry.ToRect(); // TODO: Handle null.
	}

	public override bool Matches(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return _window.ResourceClass?.Equals(opts.FileName, StringComparison.OrdinalIgnoreCase) ?? false;
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
		// if (_isAlwaysOnTop == isAlwaysOnTop)
		// {
		// 	return;
		// }

		await _kwinClient.SetWindowAlwaysOnTopAsync(_window, isAlwaysOnTop, CancellationToken.None).NoCtx();

		_isAlwaysOnTop = isAlwaysOnTop;
	}

	public override async Task SetTaskbarIconVisibleAsync(bool isVisible)
	{
		// if (_isTaskbarIconVisible == isVisible)
		// {
		// 	return;
		// }

		await _kwinClient.SetTaskbarIconVisibleAsync(_window, isVisible, CancellationToken.None).NoCtx();

		_isTaskbarIconVisible = isVisible;
	}

	public override async Task SetTransparencyAsync(int transparency)
	{
		// if (_transparency == transparency)
		// {
		// 	return;
		// }

		await _kwinClient.SetWindowOpacityAsync(_window, transparency * .01f, CancellationToken.None).NoCtx();

		_transparency = transparency;
	}

	public override async Task SetVisibleAsync(bool isVisible)
	{
		// if (_isVisible == isVisible)
		// {
		// 	return;
		// }

		await _kwinClient.SetWindowVisibleAsync(_window, isVisible, CancellationToken.None).NoCtx();

		_isVisible = isVisible;
	}

	public override Task SetWindowTitleAsync(string title)
	{
		return Task.CompletedTask;
	}
}