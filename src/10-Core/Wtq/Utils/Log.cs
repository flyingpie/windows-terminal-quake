using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;

namespace Wtq.Utils;

// TODO: Dispose or something on app close, we're dropping logs now due to lack of flush.
public static class Log
{
	private const string LogTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

	public static ILoggerFactory Factory { get; private set; }

	public static ILoggerProvider Provider { get; private set; }

	public static void Configure()
	{
		var path = WtqPaths.GetWtqLogDir();
		var logLevel = WtqEnv.LogLevel;

		var logBuilder = new LoggerConfiguration()
			.MinimumLevel.Is(logLevel)

			// In-app.
			.WriteTo.Sink(InAppLogSink.Instance)

			// JSON.
			.WriteTo.File(
				formatter: new RenderedCompactJsonFormatter(),
				path: Path.Combine(path, "logs-.json"),
				fileSizeLimitBytes: 50_000_000,
				rollingInterval: RollingInterval.Infinite,
				retainedFileCountLimit: 1)

			// Plain text.
			.WriteTo.File(
				outputTemplate: LogTemplate,
				path: Path.Combine(path, "logs-.txt"),
				fileSizeLimitBytes: 10_000_000,
				rollingInterval: RollingInterval.Day,
				retainedFileCountLimit: 5);

		// Log to console.
		var console = logBuilder.WriteTo.Console(outputTemplate: LogTemplate);

		if (Os.IsLinux && !WtqEnv.HasTermEnvVar)
		{
			Console.WriteLine("Running on Linux, and no 'TERM' environment variable found. Suggests we're called indirectly, i.e. non-interactively. Changing log level for console logger to 'warning', prevent journal spam.");

			console.MinimumLevel.Warning();
		}

		Serilog.Log.Logger = logBuilder.CreateLogger();
		Provider = new SerilogLoggerProvider(Serilog.Log.Logger);
		Factory = new SerilogLoggerFactory(Serilog.Log.Logger);
		Factory.AddProvider(Provider);

		Serilog.Log.Information("Set log level to '{Level}'", logLevel);
		Serilog.Log.Information("Logging to file at '{Path}'", path);
	}

	public static Microsoft.Extensions.Logging.ILogger For<T>()
	{
		if (Factory == null)
		{
			throw new InvalidOperationException($"Attempting to create logger for type '{typeof(T).Name}', before initializing logging.");
		}

		return Factory.CreateLogger<T>();
	}

	public static Microsoft.Extensions.Logging.ILogger For(Type type)
	{
		Guard.Against.Null(type);

		if (Factory == null)
		{
			throw new InvalidOperationException($"Attempting to create logger for type '{type.Name}', before initializing logging.");
		}

		return Factory.CreateLogger(type);
	}

	public static Microsoft.Extensions.Logging.ILogger For(string category)
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