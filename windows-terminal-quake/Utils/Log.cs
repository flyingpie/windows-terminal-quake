using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace WindowsTerminalQuake.Utils;

public static class Log
{
	private static readonly ILoggerFactory _factory;

	private static readonly Microsoft.Extensions.Logging.ILogger _log;

	static Log()
	{
		var here = Path.GetDirectoryName(new Uri(typeof(Log).Assembly.Location).LocalPath);

		Serilog.Log.Logger = new LoggerConfiguration()
			//.MinimumLevel.Is(QSettings.Instance.LogLevel)

			.WriteTo.Console()

			.WriteTo.File(
				path: Path.Combine(here, "logs/.txt"),
				fileSizeLimitBytes: 10_000_000,
				rollingInterval: RollingInterval.Day,
				retainedFileCountLimit: 3)
			.CreateLogger();

		var provider = new SerilogLoggerProvider(Serilog.Log.Logger);
		_factory = new SerilogLoggerFactory(Serilog.Log.Logger);
		_factory.AddProvider(provider);

		_log = For(typeof(Log));

		_log.LogInformation("Setting log level to '{Level}'", QSettings.Instance.LogLevel);
	}

	public static Microsoft.Extensions.Logging.ILogger For<T>()
	{
		return _factory.CreateLogger<T>();
	}

	public static Microsoft.Extensions.Logging.ILogger For(Type type)
	{
		return _factory.CreateLogger(type);
	}

	public static Microsoft.Extensions.Logging.ILogger For(string category)
	{
		return _factory.CreateLogger(category);
	}
}