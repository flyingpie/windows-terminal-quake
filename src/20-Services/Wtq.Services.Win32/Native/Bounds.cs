#pragma warning disable

namespace Wtq.Services.Win32.Native;

public struct Bounds
{
	public int Left;

	public int Top;

	public int Right;

	public int Bottom;

	public Rectangle ToRectangle() => Rectangle.FromLTRB(Left, Top, Right, Bottom);
}