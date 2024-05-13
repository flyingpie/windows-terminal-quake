using Wtq.Data;

namespace Wtq.Services;

public interface IWtqScreenInfoProvider
{
	WtqRect GetPrimaryScreenRect();

	WtqRect[] GetScreenRects();

	WtqRect GetScreenWithCursor();
}