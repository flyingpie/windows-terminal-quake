using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Wtq.Core.Data;
using Wtq.Core.Exceptions;
using Wtq.Core.Services;
using Wtq.Utils;
using Wtq.WinForms;

namespace Wtq.Win32;

public sealed class WinFormsScreenCoordsProvider : IWtqScreenCoordsProvider
{
	public WtqVec2I GetCursorPos()
	{
		return Cursor.Position.ToWtqVec2I();
	}

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
}