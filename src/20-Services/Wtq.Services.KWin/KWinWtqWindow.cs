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

	public override bool IsValid { get; }

	public override string? Name { get; }

	public override WtqRect WindowRect { get; }

	public override void BringToForeground()
	{
		// TODO
	}

	public override bool Matches(WtqAppOptions opts)
	{
		// return _window.ResourceClass?.Contains("wezterm", StringComparison.OrdinalIgnoreCase) ?? false;
		return _window.ResourceName != null && _window.ResourceName.Equals(opts.FileName, StringComparison.OrdinalIgnoreCase); // TODO
	}

	public override void MoveTo(WtqRect rect, bool repaint = true)
	{
		// TODO
		_kwinClient.MoveClientAsync(_window, rect, CancellationToken.None);
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