using Wtq.Configuration;
using Wtq.Data;
using Wtq.Services.KWin.Dto;

namespace Wtq.Services.KWin;

public class KWinWtqWindow : WtqWindow
{
	private readonly IKWinClient _kwinClient;
	private readonly KWinWindow _window;

	private bool? _isAlwaysOnTop;
	private bool? _isTaskbarIconVisible;
	private int? _transparency;
	private bool? _isVisible;

	public KWinWtqWindow(
		IKWinClient kwinClient,
		KWinWindow window)
	{
		_kwinClient = Guard.Against.Null(kwinClient);
		_window = Guard.Against.Null(window);
	}

	public override int Id { get; }

	public override bool IsValid { get; } = true;

	public override string? Name => _window?.ResourceClass;

	private WtqRect _rect;

	public override WtqRect WindowRect => _rect;

	public override async Task BringToForegroundAsync()
	{
		// TODO
		await _kwinClient.BringToForegroundAsync(_window, CancellationToken.None).ConfigureAwait(false);
	}

	public override bool Matches(WtqAppOptions opts)
	{
		var res = _window.ResourceClass?.Equals(opts.FileName, StringComparison.OrdinalIgnoreCase) ?? false;

		// return _window.ResourceName != null && _window.ResourceName.Equals(opts.FileName, StringComparison.OrdinalIgnoreCase); // TODO

		return res;
	}

	public override async Task MoveToAsync(WtqRect rect, bool repaint = true)
	{
		// TODO
		_rect = rect;
		await _kwinClient.MoveClientAsync(_window, rect, CancellationToken.None).ConfigureAwait(false);
	}

	public override async Task SetAlwaysOnTopAsync(bool isAlwaysOnTop)
	{
		if (_isAlwaysOnTop == isAlwaysOnTop)
		{
			return;
		}

		// TODO
		await _kwinClient.SetWindowAlwaysOnTopAsync(_window, isAlwaysOnTop, CancellationToken.None).ConfigureAwait(false);

		_isAlwaysOnTop = isAlwaysOnTop;
	}

	public override async Task SetTaskbarIconVisibleAsync(bool isVisible)
	{
		if (_isTaskbarIconVisible == isVisible)
		{
			return;
		}

		// TODO
		await _kwinClient.SetTaskbarIconVisibleAsync(_window, isVisible, CancellationToken.None).ConfigureAwait(false);

		_isTaskbarIconVisible = isVisible;
	}

	public override async Task SetTransparencyAsync(int transparency)
	{
		if (_transparency == transparency)
		{
			return;
		}

		// TODO
		await _kwinClient.SetWindowOpacityAsync(_window, transparency * .01f, CancellationToken.None).ConfigureAwait(false);

		_transparency = transparency;
	}

	public override async Task SetVisibleAsync(bool isVisible)
	{
		if (_isVisible == isVisible)
		{
			return;
		}

		// TODO
		await _kwinClient.SetWindowVisibleAsync(_window, isVisible, CancellationToken.None).ConfigureAwait(false);

		_isVisible = isVisible;
	}
}