using Wtq.Data;
using Wtq.Exceptions;
using Wtq.Services.Win32;
using Wtq.Utils;

namespace Wtq.Services.WinForms;

public sealed class WinFormsScreenInfoProvider : IWtqScreenInfoProvider
{
	public Task<WtqRect> GetPrimaryScreenRectAsync()
	{
		var scr = Screen.PrimaryScreen
			?? Screen.AllScreens.FirstOrDefault()
			?? throw new WtqException("No screens found!");

		return Task.FromResult(scr.Bounds.ToWtqRect());
	}

	public Task<WtqRect[]> GetScreenRectsAsync()
	{
		return Task.FromResult(Screen
			.AllScreens
			.Select(s => s.Bounds.ToWtqRect())
			.ToArray());
	}

	public async Task<WtqRect> GetScreenWithCursorAsync()
	{
		var scrs = await GetScreenRectsAsync().NoCtx();
		var c = Cursor.Position.ToWtqVec2I();

		return scrs.Any(s => s.Contains(c))
			? scrs.FirstOrDefault(s => s.Contains(c))
			: await GetPrimaryScreenRectAsync().NoCtx();
	}
}