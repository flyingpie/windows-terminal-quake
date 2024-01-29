using System.Drawing;
using Wtq.Core.Data;
using Wtq.Win32.Native;

namespace Wtq.Win32;

public static class MathExtensions
{
	public static WtqBounds ToWtqBounds(this Bounds rect)
	{
		return new WtqBounds()
		{
			Bottom = rect.Bottom,
			Left = rect.Left,
			Right = rect.Right,
			Top = rect.Top,
		};
	}

	public static WtqVec2i ToWtqVec2i(this Point point)
	{
		return new WtqVec2i()
		{
			X = point.X,
			Y = point.Y,
		};
	}

	public static WtqRect ToWtqRect(this Rectangle rect)
	{
		return new WtqRect()
		{
			Height = rect.Height,
			Width = rect.Width,
			X = rect.X,
			Y = rect.Y,
		};
	}
}