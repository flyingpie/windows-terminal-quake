namespace Wtq.Services.Stubs;

public class StubWtqScreenInfoProvider : IWtqScreenInfoProvider
{
	public Task<Rectangle> GetPrimaryScreenRectAsync()
	{
		return Task.FromResult(new Rectangle(0, 0, 1920, 1080));
	}

	public Task<Rectangle[]> GetScreenRectsAsync()
	{
		return Task.FromResult<Rectangle[]>([new Rectangle(0, 0, 1920, 1080)]);
	}

	public Task<Rectangle> GetScreenWithCursorAsync()
	{
		return Task.FromResult(new Rectangle(0, 0, 1920, 1080));
	}
}