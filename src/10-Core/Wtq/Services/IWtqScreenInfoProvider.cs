

namespace Wtq.Services;

public interface IWtqScreenInfoProvider
{
	Task<Rectangle> GetPrimaryScreenRectAsync();

	Task<Rectangle[]> GetScreenRectsAsync();

	Task<Rectangle> GetScreenWithCursorAsync();
}