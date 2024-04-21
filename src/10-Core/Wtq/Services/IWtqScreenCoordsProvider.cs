using Wtq.Data;

namespace Wtq.Services;

public interface IWtqScreenCoordsProvider
{
	WtqVec2I GetCursorPos();

	WtqRect GetPrimaryScreenRect();

	WtqRect[] GetScreenRects();
}