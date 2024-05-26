using Wtq.Configuration;
using Wtq.Data;
using Wtq.Services.KWin.Models;

namespace Wtq.Services.KWin;

public class KWinWtqWindow : WtqWindow
{
	private readonly IKWinClient _kwinClient;
	private readonly KWinWindow _window;

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

	public override void BringToForeground()
	{
		// TODO
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

	public override void SetAlwaysOnTop(bool isAlwaysOnTop)
	{
		// TODO
	}

	public override void SetTaskbarIconVisible(bool isVisible)
	{
		// TODO
	}

	public override void SetTransparency(int transparency)
	{
		// TODO
	}
}