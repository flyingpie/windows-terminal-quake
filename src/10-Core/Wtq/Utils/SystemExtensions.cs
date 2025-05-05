using System.Text.Json;

namespace Wtq.Utils;

public static class SystemExtensions
{
	public static TValue JsonDeepClone<TValue>(this TValue value)
	{
		var json = JsonSerializer.Serialize(value);

		return JsonSerializer.Deserialize<TValue>(json)!;
	}

	public static string? EmptyOrWhiteSpaceToNull(this string? input) => string.IsNullOrWhiteSpace(input) ? null : input;

	public static IEnumerable<ValidationResult> Validate(this IValidatableObject validatable)
	{
		Guard.Against.Null(validatable);

		return validatable.Validate(new ValidationContext(new object()));
	}

	public static string ExpandEnvVars(this string? src)
	{
		src ??= string.Empty;

		return Environment
				.ExpandEnvironmentVariables(src)
				?.Replace("~", WtqPaths.UserHome)
			?? string.Empty;
	}

	public static string ToSnakeCase(this string text)
	{
		if (text == null)
		{
			throw new ArgumentNullException(nameof(text));
		}

		if (text.Length < 2)
		{
			return text.ToLowerInvariant();
		}

		var sb = new StringBuilder();
		sb.Append(char.ToLowerInvariant(text[0]));
		for (int i = 1; i < text.Length; ++i)
		{
			char c = text[i];
			if (char.IsUpper(c))
			{
				sb.Append('_');
				sb.Append(char.ToLowerInvariant(c));
			}
			else
			{
				sb.Append(c);
			}
		}

		return sb.ToString();
	}
}