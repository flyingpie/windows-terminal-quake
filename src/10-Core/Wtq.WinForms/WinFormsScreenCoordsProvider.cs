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
	private readonly ILogger _log = Log.For<WinFormsScreenCoordsProvider>();

	public WtqVec2i GetCursorPos()
	{
		return Cursor.Position.ToWtqVec2i();
	}

	public WtqRect GetPrimaryScreenRect()
	{
		var scr = Screen.PrimaryScreen
			?? Screen.AllScreens.FirstOrDefault()
			?? throw new WtqException("No screens found!");

		return scr.Bounds.ToWtqRect();
	}

	public IEnumerable<WtqRect> GetScreenRects()
	{
		return Screen
			.AllScreens
			.Select(s => s.Bounds.ToWtqRect())
			.ToArray();
	}
}