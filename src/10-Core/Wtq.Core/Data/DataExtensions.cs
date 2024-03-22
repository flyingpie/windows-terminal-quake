namespace Wtq.Core.Data;

public static class DataExtensions
{
	public static WtqRect ToWtqRect(this WtqBounds bound)
		=> new()
		{
			X = bound.Left,
			Y = bound.Top,
			Width = bound.Right - bound.Left,
			Height = bound.Bottom - bound.Top,
		};
}