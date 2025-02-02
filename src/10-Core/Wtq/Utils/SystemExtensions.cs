namespace Wtq.Utils;

public static class SystemExtensions
{
	private static readonly ILogger Log = Utils.Log.For(typeof(SystemExtensions));

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

	public static string? StringJoin<T>(this IEnumerable<T> values, string separator = ", ")
	{
		ArgumentNullException.ThrowIfNull(values);

		return string.Join(separator, values);
	}

	/// <summary>
	/// Returns a rectangle from <paramref name="rects"/>, that does NOT intersect <paramref name="rectsToAvoid"/>.<br/>
	/// Returns null if no non-intersecting rectangle could be found.
	/// </summary>
	public static Rectangle? FindWithNoIntersection(this Rectangle[] rects, Rectangle[] rectsToAvoid)
	{
		Log.LogDebug("Looking for rectangle within set '{Candidates}', that does not intersect '{Rect}'", rects.StringJoin(), rectsToAvoid);

		foreach (var candidate in rects)
		{
			// if (candidate.IntersectsWith(rectsToAvoid))
			// {
			// 	Log.LogDebug("Candidate rectangle '' intersects with ");
			// }
		}

		// var targetRect = rects
		// 	.FirstOrDefault(r => !screenRects.Any(scr => scr.IntersectsWith(r)));

		return null;
	}
}