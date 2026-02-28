namespace Wtq.Utils;

public static class MathUtils
{
	/// <summary>
	/// Rounds up <paramref name="f"/>, and returns it as an integer.
	/// </summary>
	public static int CeilI(this float f) => (int)Math.Ceiling(f);

	/// <summary>
	/// Returns a <see cref="Point"/> that represents <paramref name="inner"/>, centered within <paramref name="outer"/>.<br/>
	/// https://stackoverflow.com/a/59347321.
	/// </summary>
	public static Point CenterInRectangle(this Size inner, Rectangle outer) =>
		new()
		{
			X = outer.X + ((outer.Width - inner.Width) / 2),
			Y = outer.Y + ((outer.Height - inner.Height) / 2),
		};

	/// <summary>
	/// Returns the distance between two points.
	/// </summary>
	public static float DistanceTo(this Point src, Point dst)
		=> MathF.Sqrt(MathF.Pow(dst.X - src.X, 2) + MathF.Pow(dst.Y - src.Y, 2));

	/// <summary>
	/// Linear interpolate, returns a <see cref="Point"/> that is between <paramref name="src"/> and <paramref name="dst"/>,
	/// at <paramref name="by"/> (value of 0.0f, to 1.0f).
	/// </summary>
	public static Point Lerp(Point src, Point dst, float by) =>
		new()
		{
			X = (int)float.Lerp(src.X, dst.X, by),
			Y = (int)float.Lerp(src.Y, dst.Y, by),
		};

	/// <summary>
	/// Multiply <paramref name="sz"/> by a float, truncating back to an integer <see cref="Size"/>.
	/// </summary>
	public static Size MultiplyF(this Size sz, float factor) => ((SizeF)sz * factor).ToSize();

	public static string ToShortString(this Point point) => $"({point.X}, {point.Y})";

	public static string ToShortString(this Size size) => $"{size.Width}x{size.Height}";
}