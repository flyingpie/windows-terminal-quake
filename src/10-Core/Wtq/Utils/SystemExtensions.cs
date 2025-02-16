namespace Wtq.Utils;

public static class SystemExtensions
{
	public static string ExpandEnvVars(this string src)
	{
		src ??= string.Empty;

		return Environment
			.ExpandEnvironmentVariables(src)
			?.Replace("~", WtqPaths.UserHome)
			?? string.Empty;
	}

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
}