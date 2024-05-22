using Wtq.Data;

namespace Wtq.Services.KWin;

public sealed class KWinScreenInfoProvider : IWtqScreenInfoProvider
{
	public WtqRect GetPrimaryScreenRect()
	{
		return WtqRect.Default;
	}

	public WtqRect[] GetScreenRects()
	{
		return [];
	}

	public WtqRect GetScreenWithCursor()
	{
		return WtqRect.Default;
	}
}