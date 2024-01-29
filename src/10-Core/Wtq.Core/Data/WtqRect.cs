namespace Wtq.Core.Data;

public struct WtqRect
{
	public static readonly WtqRect Default = new()
	{
		X = 100,
		Y = 100,
		Height = 1000,
		Width = 1000,
	};

	public int X { get; set; }

	public int Y { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }

	public bool Contains(WtqVec2i pos)
	{
		return
			X < pos.X &&
			Y < pos.Y &&
			X + Width > pos.X &&
			Y + Height > pos.Y;
	}
}