using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System.Globalization;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Wtq.Utils;

public static class Log
{
	private const string LogTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

	private const string LogLevelEnvVar = "WTQ_LOG_LEVEL";

	private static ILoggerFactory _factory = NullLoggerFactory.Instance;

	public static ILoggerProvider Provider { get; private set; } = NullLoggerProvider.Instance;

	/// <summary>
	/// Returns the requested log level, as specified by an environment variable.
	/// </summary>
	private static LogEventLevel LogLevel
		=> Enum.TryParse<LogEventLevel>(Environment.GetEnvironmentVariable(LogLevelEnvVar), ignoreCase: true, out var res)
			? res
			: LogEventLevel.Information;

	public static void Configure(string pathToLogsDir)
	{
		var logBuilder = new LoggerConfiguration()

			// Any logs, so we can see everything in the in-app log viewer.
			// The configured level is set per sink.
			.MinimumLevel.Is(LogEventLevel.Verbose)

			// Console
			.WriteTo.Console(
				formatProvider: CultureInfo.InvariantCulture,
				outputTemplate: LogTemplate,
				restrictedToMinimumLevel: LogLevel)

			// In-app.
			.WriteTo.Sink(InAppLogSink.Instance)

			// Plain text.
			.WriteTo.File(
				fileSizeLimitBytes: 10_000_000,
				formatProvider: CultureInfo.InvariantCulture,
				outputTemplate: LogTemplate,
				path: Path.Combine(pathToLogsDir, "logs-.txt"),
				restrictedToMinimumLevel: LogLevel,
				retainedFileCountLimit: 5,
				rollingInterval: RollingInterval.Day);

		Serilog.Log.Logger = logBuilder.CreateLogger();
		Provider = new SerilogLoggerProvider(Serilog.Log.Logger);
		_factory = new SerilogLoggerFactory(Serilog.Log.Logger);
		_factory.AddProvider(Provider);

		Serilog.Log.Information("Set log level to '{Level}'", LogLevel);
		Serilog.Log.Information("Logging to file at '{Path}'", pathToLogsDir);
	}

	public static ILogger For<T>()
	{
		return _factory.CreateLogger<T>();
	}

	public static ILogger For(Type type)
	{
		Guard.Against.Null(type);

		return _factory.CreateLogger(type);
	}

	public static ILogger For(string category)
	{
		Guard.Against.NullOrWhiteSpace(category);

		return _factory.CreateLogger(category);
	}

	public static void CloseAndFlush()
	{
		Serilog.Log.Debug("Closing logger");
		Serilog.Log.CloseAndFlush();
	}
}