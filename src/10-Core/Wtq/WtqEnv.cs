using Serilog.Events;

namespace Wtq;

public static class WtqEnv
{
	public static class Names
	{
		public const string Config = "WTQ_CONFIG_FILE";

		public const string LogLevel = "WTQ_LOG_LEVEL";
	}

	/// <summary>
	/// Returns path to the WTQ config file, as specified by an environment variable.
	/// </summary>
	public static string ConfigFile
		=> Environment.GetEnvironmentVariable(Names.Config) ?? string.Empty;

	/// <summary>
	/// Returns the requested log level, as specified by an environment variable.
	/// </summary>
	public static LogEventLevel LogLevel
		=> Enum.TryParse<LogEventLevel>(Environment.GetEnvironmentVariable(Names.LogLevel), ignoreCase: true, out var res)
		? res
		: LogEventLevel.Information;
}