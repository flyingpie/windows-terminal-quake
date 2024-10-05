using Wtq.Exceptions;
using Wtq.Utils;

namespace Wtq.Services.WinForms;

public sealed class WinFormsScreenInfoProvider : IWtqScreenInfoProvider
{
	public Task<Rectangle> GetPrimaryScreenRectAsync()
	{
		var scr = Screen.PrimaryScreen
			?? Screen.AllScreens.FirstOrDefault()
			?? throw new WtqException("No screens found!");

		return Task.FromResult(scr.Bounds);
	}

	public Task<Rectangle[]> GetScreenRectsAsync()
	{
		return Task.FromResult(Screen
			.AllScreens
			.Select(s => s.Bounds)
			.ToArray());
	}

	public async Task<Rectangle> GetScreenWithCursorAsync()
	{
		var screens = await GetScreenRectsAsync().NoCtx();
		var cursorPos = Cursor.Position;

		return screens.Any(s => s.Contains(cursorPos))
			? screens.FirstOrDefault(s => s.Contains(cursorPos))
			: await GetPrimaryScreenRectAsync().NoCtx();
	}
}