using Wtq.Data;
using Wtq.Exceptions;
using Wtq.Services.Win32;

namespace Wtq.Services.WinForms;

public sealed class WinFormsScreenInfoProvider : IWtqScreenInfoProvider
{
	public WtqRect GetPrimaryScreenRect()
	{
		var scr = Screen.PrimaryScreen
			?? Screen.AllScreens.FirstOrDefault()
			?? throw new WtqException("No screens found!");

		return scr.Bounds.ToWtqRect();
	}

	public WtqRect[] GetScreenRects()
	{
		return Screen
			.AllScreens
			.Select(s => s.Bounds.ToWtqRect())
			.ToArray();
	}

	public WtqRect GetScreenWithCursor()
	{
		var scrs = GetScreenRects();
		var c = Cursor.Position.ToWtqVec2I();

		return scrs.Any(s => s.Contains(c))
			? scrs.FirstOrDefault(s => s.Contains(c))
			: GetPrimaryScreenRect();
	}
}