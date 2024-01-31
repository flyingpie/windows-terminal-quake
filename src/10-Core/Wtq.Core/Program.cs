﻿//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Serilog;
//using Wtq.Configuration;
//using Wtq.Services;
//using Wtq.Services.AnimationTypeProviders;
//using Wtq.Services.ScreenBoundsProviders;
//using Wtq.Services.TerminalBoundsProviders;
////using Wtq.UI;

//namespace Wtq;

//public static class Program
//{
//	public static async Task Main(string[] args)
//	{
//		Console.WriteLine("Hello, World!");

//		// Configuration.
//		var config = new ConfigurationBuilder()
//			.AddJsonFile(f =>
//			{
//				f.Optional = false;
//				f.Path = "windows-terminal-quake.jsonc";
//			})
//			.Build();

//		// Logging.
//		Wtq.Utils.Log.Configure(config);

//		await new HostBuilder()
//			.ConfigureAppConfiguration(opt =>
//			{
//				opt.AddConfiguration(config);
//			})
//			.ConfigureServices(opt =>
//			{
//				opt
//					.AddOptionsWithValidateOnStart<WtqOptions>()
//					.Bind(config);

//				opt
//					.AddSingleton<IAnimationProvider, AnimationProvider>()
//					.AddSingleton<IScreenBoundsProvider, ScreenWithCursorScreenBoundsProvider>()
//					.AddSingleton<ITerminalBoundsProvider, MovingTerminalBoundsProvider>()
//					.AddSingleton<IWtqProcessFactory, WtqProcessFactory>()

//					.AddSingleton<IToggler, WtqToggler>()
//					.AddSingleton<IRetry, Retry>()
//					.AddSingleton<WtqAppMonitorService>()

//					.AddHostedService(p => p.GetRequiredService<WtqAppMonitorService>())
//					//.AddHostedService<GlobalHotkeyService>()
//					//.AddHostedService<TrayIconService>()
//					.AddHostedService<WtqService>();
//			})
//			.UseSerilog()
//			.Build()
//			.RunAsync();
//	}
//}