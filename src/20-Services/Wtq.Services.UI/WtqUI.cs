using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Photino.Blazor;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Wtq.Services.UI;

public sealed class WtqUI
	: IHostedService
{
	private readonly IWtqWindowService _processService;
	private Thread? _uiThread;

	public WtqUI(IWtqWindowService processService)
	{
		_processService = processService;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_uiThread = new Thread(StartUI);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			_uiThread.SetApartmentState(ApartmentState.STA);
		}

		_uiThread.Start();

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		//_uiThread.Join();

		return Task.CompletedTask;
	}

	private void StartUI()
	{
		var appBuilder = PhotinoBlazorAppBuilder.CreateDefault();


		appBuilder.Services
			.AddSingleton<IWtqWindowService>(p => _processService)
			.AddLogging();

		// register root component
		appBuilder.RootComponents.Add<App>("app");

		var app = appBuilder.Build();

		// customize window
		app.MainWindow
			// .SetIconFile("favicon.ico")
			.SetTitle("Photino Hello World");

		app.MainWindow.RegisterWindowClosingHandler((s, a) =>
		{
			var dbg = 2;

			return false;
		});

		AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
		{
			app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
		};

		app.Run();

		var dbg2 = 2;
	}
}