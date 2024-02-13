using Wtq.Core.Data;

namespace Wtq.Core.Services;

public interface IWtqScreenCoordsProvider
{
	WtqVec2i GetCursorPos();

	WtqRect GetPrimaryScreenRect();

	WtqRect[] GetScreenRects();
}