using WindowsTerminalQuake.Native;

namespace WindowsTerminalQuake.Extensions;

public static class MathExtensions
{
	public static Rectangle ToRectangle(this Rect rect)
		=> new(
			x: rect.Left,
			y: rect.Top,
			width: rect.Right - rect.Left,
			height: rect.Bottom - rect.Top);
}