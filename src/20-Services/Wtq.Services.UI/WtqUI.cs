using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Photino.Blazor;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Wtq.Utils;

namespace Wtq.Services.UI;

public sealed class WtqUI : IHostedService, IWtqUIService
{
	private readonly IHostApplicationLifetime _appLifetime;
	private readonly IWtqWindowService _windowService;
	private readonly IWtqWindowService _processService;

	private Thread? _uiThread;

	private PhotinoBlazorApp? _app;
	private Point? _loc;

	public WtqUI(
		IHostApplicationLifetime appLifetime,
		IWtqWindowService windowService,
		IWtqWindowService processService)
	{
		_appLifetime = Guard.Against.Null(appLifetime);
		_windowService = Guard.Against.Null(windowService);
		_processService = Guard.Against.Null(processService);

		_uiThread = new Thread(StartUI);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			_uiThread.SetApartmentState(ApartmentState.STA);
		}

		_uiThread.Start();
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public async Task CloseMainWindowAsync()
	{
		var w = await FindWtqMainWindowAsync().NoCtx();

		if (w == null)
		{
			return;
		}

		await w.MoveToAsync(new Point(0, -1_000_000)).NoCtx();
		await w.SetTaskbarIconVisibleAsync(false).NoCtx();
	}

	public async Task OpenMainWindowAsync()
	{
		var w = await FindWtqMainWindowAsync().NoCtx();

		if (w == null)
		{
			return;
		}

		await w.MoveToAsync(_loc ?? Point.Empty).NoCtx();
		await w.BringToForegroundAsync().NoCtx();
		await w.SetTaskbarIconVisibleAsync(true).NoCtx();
	}

	public void RunOnUIThread(Action action)
	{
		_app?.MainWindow?.Invoke(action);
	}

	private async Task<WtqWindow?> FindWtqMainWindowAsync()
	{
		for (var i = 0; i < 10; i++)
		{
			var windows = await _windowService.GetWindowsAsync().NoCtx();

			var mainWindow = windows.FirstOrDefault(w => w.Title == "WTQ - Main Window");

			if (mainWindow != null)
			{
				_loc ??= (await mainWindow.GetWindowRectAsync().NoCtx()).Location;
				return mainWindow;
			}

			await Task.Delay(TimeSpan.FromMilliseconds(200)).NoCtx();
		}

		return null;
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
			.SetIconFile("icon.png")
			.SetTitle("WTQ - Main Window");

		_app.MainWindow.RegisterWindowCreatedHandler(
			(s, a) =>
			{
				_ = Task.Run(CloseMainWindowAsync);
			});

		_app.MainWindow.RegisterWindowClosingHandler(
			(s, a) =>
			{
				_ = Task.Run(CloseMainWindowAsync);

				return true;
			});

		_appLifetime.ApplicationStopping.Register(() => _app.MainWindow.Close());

		_app.Run();
	}
}