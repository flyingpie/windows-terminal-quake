using Wtq.Configuration;
using Wtq.Data;

namespace Wtq.Services.KWin;

public class KWinWtqWindow : WtqWindow
{
	public override int Id { get; }

	public override bool IsValid { get; }

	public override string? Name { get; }

	public override WtqRect WindowRect { get; }

	public override void BringToForeground()
	{

	}

	public override bool Matches(WtqAppOptions opts)
	{
		return false; // TODO
	}

	public override void MoveTo(WtqRect rect, bool repaint = true)
	{
		// TODO
	}

	public override void SetAlwaysOnTop(bool isAlwaysOnTop)
	{

	}

	public override void SetTaskbarIconVisible(bool isVisible)
	{

	}

	public override void SetTransparency(int transparency)
	{

	}
}