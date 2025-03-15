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
}