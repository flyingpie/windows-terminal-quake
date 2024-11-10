using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using System.Runtime.InteropServices;
using Wtq.Utils.AsyncInit;

namespace Wtq.Services.UI;

public sealed class WtqUI(IWtqWindowService processService)
	: IAsyncInitializable
{
	private readonly IWtqWindowService _processService = Guard.Against.Null(processService);

	private Thread? _uiThread;
	private PhotinoBlazorApp? _app;

	public Task InitializeAsync()
	{
		return Task.CompletedTask;
	}

	public void OpenMainWindow()
	{
		_uiThread = new Thread(StartUI);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			_uiThread.SetApartmentState(ApartmentState.STA);
		}

		_uiThread.Start();
	}

	private void StartUI()
	{
		var appBuilder = PhotinoBlazorAppBuilder.CreateDefault();

		// TODO: Unify with the main app DI.
		appBuilder.Services
			.AddSingleton<IWtqWindowService>(p => _processService)
			.AddUI()
			.AddLogging();

		appBuilder.RootComponents.Add<App>("app");

		_app = appBuilder.Build();

		_app.MainWindow

			// .SetIconFile("wwwroot/img/icon.ico")
			.SetTitle("Photino Hello World 3");

		_app.MainWindow.RegisterWindowCreatedHandler(
			(s, a) =>
			{
				//
				Console.WriteLine("CREATED");
			});

		_app.MainWindow.RegisterWindowClosingHandler(
			(s, a) =>
			{
				var dbg = 2;
				_app = null;

				return false;
			});

		AppDomain.CurrentDomain.UnhandledException +=
			(sender, error) =>
			{
				//
				_app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
			};

		_app.Run();

		Console.WriteLine("CLOSE");
	}
}