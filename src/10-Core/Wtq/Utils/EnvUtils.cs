namespace Wtq.Utils;

public static class EnvUtils
{
	private static string UserHome => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

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
				?.Replace("~", UserHome)
			?? string.Empty;
	}

	public static string GetEnvVarOrDefault(string name, string defaultValue)
	{
		Guard.Against.NullOrWhiteSpace(name);
		Guard.Against.NullOrWhiteSpace(defaultValue);

		return GetEnvVar(name) ?? defaultValue;
	}

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
}