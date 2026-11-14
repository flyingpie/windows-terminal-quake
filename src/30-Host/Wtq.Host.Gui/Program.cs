using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using Radzen;
using Wtq.Host.Base;
using Wtq.Services;

namespace Wtq.Host.Gui;

internal static class Program
{
	[STAThread]
	private static void Main(string[] args)
	{
		var appBuilder = PhotinoBlazorApp.CreateBuilder(args);
		var platform = PlatformServiceFactory.Create();

		// Register the root component for the main window.
		appBuilder.RootComponents.Add<App>("app");

		appBuilder
			.Services
			//
			.AddConfiguration(platform)
			.AddLogging()
			.AddRadzenComponents()
			.AddSingleton(platform)
			.AddSingleton<IWtqOptionsSaveService, WtqOptionsSaveService>()
			.AddTransient<Notifier>();

		using var app = appBuilder.Build();

		// Customize the native Photino window.
		app.MainWindow
			//
			.Center()
			.SetIconFile(Path.Combine(platform.PathToAssetsDir, "icon-v2-256-padding.png"))
			.SetJavascriptClipboardAccessEnabled(true)
			.SetLogVerbosity(0)
			.SetSize(1270, 800)
			.SetTitle("WTQ - Main Window");

		app.Run();
	}
}
