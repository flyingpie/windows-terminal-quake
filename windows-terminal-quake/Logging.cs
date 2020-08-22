﻿using Serilog;
using System;
using System.IO;

namespace WindowsTerminalQuake
{
	public static class Logging
	{
		public static void Configure()
		{
			var here = Path.GetDirectoryName(new Uri(typeof(Logging).Assembly.Location).AbsolutePath);

			var builder = new LoggerConfiguration()
				.MinimumLevel.Is(Serilog.Events.LogEventLevel.Information)
			;

			if (Settings.Instance.Logging)
			{
				builder.WriteTo.File(
					path: Path.Combine(here, "logs/.txt"),
					fileSizeLimitBytes: 10_000_000,
					rollingInterval: RollingInterval.Day,
					retainedFileCountLimit: 3
				);
			}

			Log.Logger = builder.CreateLogger();

			Log.Information("Windows Terminal Quake started");
		}
	}
}