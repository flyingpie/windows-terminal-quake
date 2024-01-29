using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;
using Wtq.Configuration;
using Wtq.Core.Service;
using Wtq.Core.Services;
using Wtq.Services;
using Wtq.Services.AnimationTypeProviders;
using Wtq.Services.ScreenBoundsProviders;
using Wtq.Services.TerminalBoundsProviders;
using Wtq.SharpHook;
using Wtq.SimpleTrayIcon;
using Wtq.Utils;
using Wtq.Win32;
using Wtq.WinFomsrms;

namespace Wtq;

public static class Program
{
	public static async Task Main(string[] args)
	{
		Console.WriteLine("Hello, World!");

		// Configuration.
		var config = new ConfigurationBuilder()
			.AddJsonFile(f =>
			{
				f.Optional = false;
				f.Path = "wtq.jsonc";
			})
			.Build();

		// Logging.
		Wtq.Utils.Log.Configure(config);

		await new HostBuilder()
			.ConfigureAppConfiguration(opt =>
			{
				opt.AddConfiguration(config);
			})
			.ConfigureServices(opt =>
			{
				opt
					.AddOptionsWithValidateOnStart<WtqOptions>()
					.Bind(config);

				opt
					// Utils
					.AddSingleton<IRetry, Retry>()

					// Core App Logic
					.AddSingleton<IAnimationProvider, AnimationProvider>()
					.AddSingleton<IScreenBoundsProvider, ScreenWithCursorScreenBoundsProvider>()
					.AddSingleton<ITerminalBoundsProvider, MovingTerminalBoundsProvider>()
					.AddSingleton<IWtqProcessFactory, WtqProcessFactory>()
					.AddSingleton<IWtqAppToggleService, WtqAppToggleService>()
					.AddSingleton<WtqAppMonitorService>()
					.AddSingleton<IWtqBus, WtqBus>()
					.AddHostedService(p => p.GetRequiredService<WtqAppMonitorService>())
					.AddHostedService<WtqService>()
					.AddSingleton<IWtqAppRepo, WtqAppRepo>()
					.AddHostedService<WtqHotkeyService>()

					// Platform-specific
					//.AddSingleton<IWtqProcessService, Win32ProcessService>()
					//.AddSingleton<IWtqScreenCoordsProvider, WinFormsScreenCoordsProvider>()
					.AddWin32ProcessService()
					.AddWinFormsScreenCoordsProvider()
					.AddWinFormsHotkeyService()
					//.AddHostedService<SharpHookGlobalHotkeyService>()
					//.AddHostedService<SimpleTrayIconService>()
					//.AddSharpHookGlobalHotkeys()
					.AddSimpleTrayIcon()
					;
			})
			.UseSerilog()
			.Build()
			.RunAsync();
	}
}