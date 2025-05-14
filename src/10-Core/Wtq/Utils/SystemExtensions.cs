using System.Text.Json;

namespace Wtq.Utils;

public static class SystemExtensions
{
	public static TValue JsonDeepClone<TValue>(this TValue value)
	{
		var json = JsonSerializer.Serialize(value);

		return JsonSerializer.Deserialize<TValue>(json)!;
	}

	/// <summary>
	/// "" => null<br/>
	/// " " => null<br/>
	/// "\t" => null<br/>
	/// "\n" => null<br/>
	/// "not-null" => "not-null".<br/>
	/// </summary>
	public static string? EmptyOrWhiteSpaceToNull(this string? input) => string.IsNullOrWhiteSpace(input) ? null : input;

	public static IEnumerable<ValidationResult> Validate(this IValidatableObject validatable)
	{
		Guard.Against.Null(validatable);

		return validatable.Validate(new ValidationContext(new object()));
	}

	/// <summary>
	/// Replace variables such as "%ENV_VAR%".<br/>
	/// E.g. "User %USER% is logged in" => "User username1 is logged in".<br/>
	/// Also replaces "~" with the path to the user's home directory.
	/// </summary>
	public static string ExpandEnvVars(this string source)
	{
		Guard.Against.Null(source);

		return Environment
				.ExpandEnvironmentVariables(source)
				?.Replace("~", WtqPaths.UserHome)
			?? string.Empty;
	}

	/// <summary>
	/// "ToSnakeCase" => "to_snake_case".
	/// </summary>
	public static string ToSnakeCase(this string source)
	{
		Guard.Against.Null(source);

		if (source.Length <= 1)
		{
			return source.ToLowerInvariant();
		}

		var sb = new StringBuilder();
		sb.Append(char.ToLowerInvariant(source[0]));
		for (var i = 1; i < source.Length; ++i)
		{
			var c = source[i];
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