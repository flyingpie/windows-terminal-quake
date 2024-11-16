using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Photino.Blazor;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Utils;

namespace Wtq.Services.UI;

public sealed class WtqDisqUI
{
	private readonly IHostApplicationLifetime _appLifetime;
	private readonly IWtqWindowService _windowService;
	private readonly IWtqWindowService _processService;

	private PhotinoBlazorApp? _app;
	private Point? _loc;

	public WtqDisqUI(
		IHostApplicationLifetime appLifetime,
		IWtqBus bus,
		IWtqWindowService windowService,
		IWtqWindowService processService)
	{
		_ = Guard.Against.Null(bus);
		_appLifetime = Guard.Against.Null(appLifetime);
		_windowService = Guard.Against.Null(windowService);
		_processService = Guard.Against.Null(processService);

//		bus.OnEvent<WtqUIRequestedEvent>(e => OpenMainWindowAsync());

		// bus.OnEvent<WtqHotkeyPressedEvent>(e => e.Key == Keys.D2, e => _isOpen ? CloseMainWindowAsync() : OpenMainWindowAsync());
	}

	private bool _isOpen;

	public async Task CloseMainWindowAsync()
	{
		var w = await FindWtqMainWindowAsync().NoCtx();

		if (w == null)
		{
			return;
		}

		await w.MoveToAsync(new Point(0, -1_000_000)).NoCtx();
		await w.SetTaskbarIconVisibleAsync(false).NoCtx();

		_isOpen = false;
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

		_isOpen = true;
	}

	private async Task<WtqWindow?> FindWtqMainWindowAsync()
	{
		for (var i = 0; i < 10; i++)
		{
			var windows = await _windowService.GetWindowsAsync(CancellationToken.None).NoCtx();

			var mainWindow = windows.FirstOrDefault(w => w.Title == "WTQ - Disq");

			if (mainWindow != null)
			{
				_loc ??= (await mainWindow.GetWindowRectAsync().NoCtx()).Location;
				return mainWindow;
			}

			await Task.Delay(TimeSpan.FromMilliseconds(200)).NoCtx();
		}

		return null;
	}

	private bool _isClosing;

	public void StartUI()
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
			.SetSize(500, 300)
			.SetIconFile(WtqPaths.GetPathRelativeToWtqAppDir("icon-v2-64.png"))
			.SetTitle("WTQ - Disq");

		_app.WindowManager.Navigate("/disq");

		_app.MainWindow.RegisterWindowCreatedHandler(
			(s, a) =>
			{
				_ = Task.Run(CloseMainWindowAsync);
			});

		_app.MainWindow.RegisterWindowClosingHandler(
			(s, a) =>
			{
				_ = Task.Run(CloseMainWindowAsync);

				return !_isClosing;
			});

		_appLifetime.ApplicationStopping.Register(() =>
		{
			_isClosing = true;

			_app.MainWindow.Close();
		});

		_app.Run();
	}
}