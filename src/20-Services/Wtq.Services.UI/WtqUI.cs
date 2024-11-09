using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Photino.Blazor;
using System.Runtime.InteropServices;

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
		// _uiThread.Join();

		return Task.CompletedTask;
	}

	private void StartUI()
	{
		var appBuilder = PhotinoBlazorAppBuilder.CreateDefault();

		// TODO: Unify with the main app DI.
		appBuilder.Services
			.AddSingleton<IWtqWindowService>(p => _processService)
			.AddUI()
			.AddLogging();

		// register root component
		appBuilder.RootComponents.Add<App>("app");

		var app = appBuilder.Build();

		// customize window
		app.MainWindow
			.SetIconFile("wwwroot/img/icon.ico")
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