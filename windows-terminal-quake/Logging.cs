using Serilog;
using System;
using System.IO;
using WindowsTerminalQuake.Settings;

namespace WindowsTerminalQuake
{
	public static class Logging
	{
		public static void Configure()
		{
			var here = Path.GetDirectoryName(new Uri(typeof(Logging).Assembly.Location).LocalPath);

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Is(QSettings.Instance.LogLevel)
				.WriteTo.File(
					path: Path.Combine(here, "logs/.txt"),
					fileSizeLimitBytes: 10_000_000,
					rollingInterval: RollingInterval.Day,
					retainedFileCountLimit: 3
				)
				.CreateLogger()
			;

			Log.Information($"Setting log level to '{QSettings.Instance.LogLevel}'.");
		}
	}
}