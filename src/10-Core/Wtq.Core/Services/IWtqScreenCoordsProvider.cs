using Wtq.Core.Data;

namespace Wtq.Core.Services;

public interface IWtqScreenCoordsProvider
{
	WtqVec2I GetCursorPos();

	WtqRect GetPrimaryScreenRect();

	WtqRect[] GetScreenRects();
}