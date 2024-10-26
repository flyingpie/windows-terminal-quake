namespace Wtq.Utils;

public static class MathUtils
{
	/// <summary>
	/// Linear interpolate, returns a <see cref="Point"/> that is between <paramref name="src"/> and <paramref name="dst"/>, at <paramref name="by"/> (value of 0.0f, to 1.0f).
	/// </summary>
	public static Point Lerp(Point src, Point dst, float by) =>
		new()
		{
			X = (int)float.Lerp(src.X, dst.X, by),
			Y = (int)float.Lerp(src.Y, dst.Y, by),
		};
}