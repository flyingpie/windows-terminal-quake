namespace Wtq.Utils;

public static class EnvUtils
{
	/// <summary>
	/// Returns the absolute path to the user home.<br/>
	/// Linux:    "/home/username".<br/>
	/// Windows:  "C:/users/username".
	/// </summary>
	private static string UserHome => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

	/// <summary>
	/// Replace variables such as "%ENV_VAR%".<br/>
	/// E.g. "User %USER% is logged in" => "User username1 is logged in".<br/>
	/// Also replaces "~" with the path to the user's home directory.<br/>
	/// TODO: Currently case-sensitive, kinda wanna fix that.
	/// </summary>
	public static string ExpandEnvVars(this string source)
	{
		Guard.Against.Null(source);

		return Environment
			.ExpandEnvironmentVariables(source)
			?.Replace("~", UserHome) ?? string.Empty;
	}

	/// <summary>
	/// Returns the value of the environment variable with the specified <paramref name="name"/>.<br/>
	/// If none exists, or its value is empty, the value of <paramref name="defaultValue"/> is returned instead.
	/// </summary>
	public static string GetEnvVarOrDefault(string name, string defaultValue)
	{
		Guard.Against.NullOrWhiteSpace(name);
		Guard.Against.NullOrWhiteSpace(defaultValue);

		return GetEnvVar(name) ?? defaultValue;
	}

	/// <summary>
	/// Returns the value of the environment variable with the specified <paramref name="name"/>.
	/// </summary>
	public static string? GetEnvVar(string name)
	{
		Guard.Against.NullOrWhiteSpace(name);

		return Environment
			.GetEnvironmentVariables()
			.Cast<DictionaryEntry>()
			.FirstOrDefault(d => d.Key?.ToString()?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false)
			.Value
			?.ToString()
			?.Trim()
			?.ExpandEnvVars()
			?.EmptyOrWhiteSpaceToNull();
	}

	/// <summary>
	/// Returns whether the value of the environment variable with name <paramref name="name"/> equals <paramref name="value"/>.<br/>
	/// Case-insensitive.
	/// </summary>
	public static bool HasEnvVarWithValue(string name, string value)
	{
		Guard.Against.NullOrWhiteSpace(name);
		Guard.Against.NullOrWhiteSpace(value);

		return GetEnvVar(name)?.Equals(value, StringComparison.OrdinalIgnoreCase) ?? false;
	}
}