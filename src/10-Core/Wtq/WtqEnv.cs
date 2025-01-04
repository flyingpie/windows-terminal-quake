using Serilog.Events;

namespace Wtq;

public static class WtqEnv
{
	/// <summary>
	/// Returns path to the WTQ config file, as specified by an environment variable.
	/// </summary>
	public static string ConfigFileFromEnv
		=> Environment.GetEnvironmentVariable(Names.Config) ?? string.Empty;

	/// <summary>
	/// Returns the requested log level, as specified by an environment variable.
	/// </summary>
	public static LogEventLevel LogLevelFromEnv
		=> Enum.TryParse<LogEventLevel>(Environment.GetEnvironmentVariable(Names.LogLevel), ignoreCase: true, out var res)
		? res
		: LogEventLevel.Information;

	public static class Names
	{
		public const string Config = "WTQ_CONFIG_FILE";

		public const string LogLevel = "WTQ_LOG_LEVEL";
	}
}