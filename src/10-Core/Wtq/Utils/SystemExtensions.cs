namespace Wtq.Utils;

public static class SystemExtensions
{
	public static int DistanceTo(this Point src, Point dst)
		=> (int)Math.Sqrt(Math.Pow(dst.X - src.X, 2) + Math.Pow(dst.Y - src.Y, 2));

	public static string? EmptyOrWhiteSpaceToNull(this string? input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return null;
		}

		return input;
	}
}