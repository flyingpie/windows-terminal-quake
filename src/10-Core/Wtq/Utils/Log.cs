#pragma warning disable

using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Extensions.Logging;

namespace Wtq.Utils;

public static class Log
{
	private static ILoggerFactory _factory;

	public static void Configure(IConfiguration configuration)
	{
		Serilog.Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Verbose()

			.WriteTo.Console()

			.WriteTo.File(
				path: Path.Combine(AppPaths.PathToAppDir, "logs", ".txt"),
				fileSizeLimitBytes: 10_000_000,
				rollingInterval: RollingInterval.Day,
				retainedFileCountLimit: 3)
			.CreateLogger();

		var provider = new SerilogLoggerProvider(Serilog.Log.Logger);
		_factory = new SerilogLoggerFactory(Serilog.Log.Logger);
		_factory.AddProvider(provider);
	}

	public static Microsoft.Extensions.Logging.ILogger For<T>()
	{
		if (_factory == null)
		{
			throw new InvalidOperationException($"Attempting to create logger for type '{typeof(T).Name}', before initializing logging.");
		}

		return _factory.CreateLogger<T>();
	}

	public static Microsoft.Extensions.Logging.ILogger For(Type type)
	{
		Guard.Against.Null(type);

		if (_factory == null)
		{
			throw new InvalidOperationException($"Attempting to create logger for type '{type.Name}', before initializing logging.");
		}

		return _factory.CreateLogger(type);
	}

	public static Microsoft.Extensions.Logging.ILogger For(string category)
	{
		Guard.Against.NullOrWhiteSpace(category);

		if (_factory == null)
		{
			throw new InvalidOperationException($"Attempting to create logger for category '{category}', before initializing logging.");
		}

		return _factory.CreateLogger(category);
	}
}