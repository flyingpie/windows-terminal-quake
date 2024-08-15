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
	private Rectangle _rect;

	public override int Id { get; }

	public override bool IsValid { get; } = true;

	public override string? Name => _window?.ResourceClass;

	public override Rectangle WindowRect => _rect;

	public override async Task BringToForegroundAsync()
	{
		await _kwinClient.BringToForegroundAsync(_window, CancellationToken.None).NoCtx();
	}

	public override bool Matches(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		var res = _window.ResourceClass?.Equals(opts.FileName, StringComparison.OrdinalIgnoreCase) ?? false;

		// return _window.ResourceName != null && _window.ResourceName.Equals(opts.FileName, StringComparison.OrdinalIgnoreCase); // TODO

		return res;
	}

	public override async Task MoveToAsync(Rectangle rect, bool repaint = true)
	{
		// TODO
		_rect = rect;
		await _kwinClient.MoveClientAsync(_window, rect, CancellationToken.None).NoCtx();
	}

	public override async Task SetAlwaysOnTopAsync(bool isAlwaysOnTop)
	{
		if (_isAlwaysOnTop == isAlwaysOnTop)
		{
			return;
		}

		// TODO
		await _kwinClient.SetWindowAlwaysOnTopAsync(_window, isAlwaysOnTop, CancellationToken.None).NoCtx();

		_isAlwaysOnTop = isAlwaysOnTop;
	}

	public override async Task SetTaskbarIconVisibleAsync(bool isVisible)
	{
		if (_isTaskbarIconVisible == isVisible)
		{
			return;
		}

		// TODO
		await _kwinClient.SetTaskbarIconVisibleAsync(_window, isVisible, CancellationToken.None).NoCtx();

		_isTaskbarIconVisible = isVisible;
	}

	public override async Task SetTransparencyAsync(int transparency)
	{
		if (_transparency == transparency)
		{
			return;
		}

		// TODO
		await _kwinClient.SetWindowOpacityAsync(_window, transparency * .01f, CancellationToken.None).NoCtx();

		_transparency = transparency;
	}

	public override async Task SetVisibleAsync(bool isVisible)
	{
		if (_isVisible == isVisible)
		{
			return;
		}

		// TODO
		await _kwinClient.SetWindowVisibleAsync(_window, isVisible, CancellationToken.None).NoCtx();

		_isVisible = isVisible;
	}
}