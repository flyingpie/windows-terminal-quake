using Wtq.Data;

namespace Wtq.Services;

public interface IWtqScreenInfoProvider
{
	Task<WtqRect> GetPrimaryScreenRectAsync();

	Task<WtqRect[]> GetScreenRectsAsync();

	Task<WtqRect> GetScreenWithCursorAsync();
}