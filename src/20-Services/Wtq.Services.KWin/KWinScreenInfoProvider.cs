using Wtq.Data;

namespace Wtq.Services.KWin;

public sealed class KWinScreenInfoProvider : IWtqScreenInfoProvider
{
	public WtqRect GetPrimaryScreenRect()
	{
		return new WtqRect()
		{
			X = 0,
			Y = 0,
			Width = 1920,
			Height = 1080,
		};
	}

	public WtqRect[] GetScreenRects()
	{
		return [GetPrimaryScreenRect()];
	}

	public WtqRect GetScreenWithCursor()
	{
		return GetPrimaryScreenRect();
	}
}