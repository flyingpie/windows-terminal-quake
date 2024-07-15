using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using System;

namespace Wtq.WtqUI;

public static class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		new WtqUI().Start(args);
	}
}

public sealed class WtqUI
{
	public void Start(string[] args)
	{
		var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);
		appBuilder.Services
			.AddLogging();

		// register root component
		appBuilder.RootComponents.Add<App>("app");

		var app = appBuilder.Build();

		// customize window
		app.MainWindow
			// .SetIconFile("favicon.ico")
			.SetTitle("Photino Hello World");

		AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
		{
			app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
		};

		app.Run();
	}
}