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
using Wtq.SimpleTrayIcon;
using Wtq.Utils;
using Wtq.Win32;
using Wtq.Win32.Native;
using Wtq.Windows;
using Wtq.WinForms;

namespace Wtq;

public static class Program
{
	public static async Task Main(string[] args)
	{
		//Kernel32.AllocConsole();

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
		Utils.Log.Configure(config);

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

					.AddSingletonHostedService<IWtqFocusTracker, WtqFocusTracker>()

					// Platform-specific.
					.AddWin32ProcessService()
					.AddWinFormsScreenCoordsProvider()
					.AddWinFormsHotkeyService()
					.AddWinFormsTrayIcon()
					//.AddSharpHookGlobalHotkeys()
					//.AddSimpleTrayIcon()
					;
			})
			.UseSerilog()
			.Build()
			.RunAsync();
	}
}