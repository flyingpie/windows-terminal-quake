namespace Wtq.Utils;

public static class SystemExtensions
{
	public static string? EmptyOrWhiteSpaceToNull(this string? input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return null;
		}

		return input;
	}
}