using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Wtq.Utils;

// TODO: Dispose or something on app close, we're dropping logs now due to lack of flush.
public static class Log
{
	private const string LogTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

	public const string LogLevelEnvVar = "WTQ_LOG_LEVEL";

	public static ILoggerFactory Factory { get; private set; }

	public static ILoggerProvider Provider { get; private set; }

	/// <summary>
	/// Returns the requested log level, as specified by an environment variable.
	/// </summary>
	public static LogEventLevel LogLevel
		=> Enum.TryParse<LogEventLevel>(Environment.GetEnvironmentVariable(LogLevelEnvVar), ignoreCase: true, out var res)
			? res
			: LogEventLevel.Information;

	public static void Configure(string pathToLogsDir)
	{
		var logLevel = LogLevel;

		var logBuilder = new LoggerConfiguration()
			.MinimumLevel.Is(logLevel)

			// In-app.
			.WriteTo.Sink(InAppLogSink.Instance)

			// Plain text.
			.WriteTo.File(
				outputTemplate: LogTemplate,
				path: Path.Combine(pathToLogsDir, "logs-.txt"),
				fileSizeLimitBytes: 10_000_000,
				rollingInterval: RollingInterval.Day,
				retainedFileCountLimit: 5);

		// Log to console.
		var console = logBuilder.WriteTo.Console(outputTemplate: LogTemplate);

		Serilog.Log.Logger = logBuilder.CreateLogger();
		Provider = new SerilogLoggerProvider(Serilog.Log.Logger);
		Factory = new SerilogLoggerFactory(Serilog.Log.Logger);
		Factory.AddProvider(Provider);

		Serilog.Log.Information("Set log level to '{Level}'", logLevel);
		Serilog.Log.Information("Logging to file at '{Path}'", pathToLogsDir);
	}

	public static ILogger For<T>()
	{
		if (Factory == null)
		{
			throw new InvalidOperationException($"Attempting to create logger for type '{typeof(T).Name}', before initializing logging.");
		}

		return Factory.CreateLogger<T>();
	}

	public static ILogger For(Type type)
	{
		Guard.Against.Null(type);

		if (Factory == null)
		{
			throw new InvalidOperationException($"Attempting to create logger for type '{type.Name}', before initializing logging.");
		}

		return Factory.CreateLogger(type);
	}

	public static ILogger For(string category)
	{
		Guard.Against.NullOrWhiteSpace(category);

		if (Factory == null)
		{
			throw new InvalidOperationException($"Attempting to create logger for category '{category}', before initializing logging.");
		}

		return Factory.CreateLogger(category);
	}

	public static void CloseAndFlush()
	{
		Serilog.Log.Debug("Closing logger");
		Serilog.Log.CloseAndFlush();
	}
}