using Microsoft.Extensions.Hosting;
using Photino.Blazor;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Wtq.Services.UI;

public class WtqUIHost
{
	private const string MainWindowTitle = "WTQ - Main Window";

	private readonly IWtqWindowService _windowService;
	private readonly IWtqScreenInfoProvider _scrInfoProvider;
	private readonly PhotinoBlazorApp _app;
	private bool _isClosing;

	public WtqUIHost(
		IWtqBus bus,
		IWtqWindowService windowService,
		IWtqScreenInfoProvider scrInfoProvider,
		IHostApplicationLifetime appLifetime,
		IEnumerable<IHostedService> hostedServices,
		PhotinoBlazorApp app)
	{
		_windowService = windowService;
		_scrInfoProvider = scrInfoProvider;
		_app = app;

		bus.OnEvent<WtqUIRequestedEvent>(e => OpenMainWindowAsync());

		app.MainWindow.RegisterWindowClosingHandler(
			(s, a) =>
			{
				_ = Task.Run(CloseMainWindowAsync);

				return !_isClosing;
			});

		appLifetime.ApplicationStarted.Register(() =>
		{
			Task
				.Run(async () =>
				{
					foreach (var xx in hostedServices)
					{
						await xx.StartAsync(CancellationToken.None);
					}
				})
				.GetAwaiter()
				.GetResult();
		});

		appLifetime.ApplicationStopping.Register(
			() =>
			{
				Task
					.Run(async () =>
					{
						foreach (var xx in hostedServices)
						{
							await xx.StopAsync(CancellationToken.None);
						}
					})
					.GetAwaiter()
					.GetResult();

				_isClosing = true;

				app.MainWindow.Close();

				Task
					.Run(async () =>
					{
						foreach (var xx in hostedServices.OfType<IAsyncDisposable>())
						{
							await xx.DisposeAsync();
						}
					})
					.GetAwaiter()
					.GetResult();
			});

		app.MainWindow
			.SetLogVerbosity(0)
			.SetIconFile(WtqPaths.GetPathRelativeToWtqAppDir("assets", "icon-v2-64.png"))
			.SetTitle(MainWindowTitle);
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

		// await w.MoveToAsync(_loc ?? Point.Empty).NoCtx();
		await w.BringToForegroundAsync().NoCtx();
		await w.SetTaskbarIconVisibleAsync(true).NoCtx();

		// TODO: Replace with screen info provider thing.
		_app.MainWindow.Center();

		var scr = await _scrInfoProvider.GetScreenWithCursorAsync().NoCtx();
		scr.GetType();
	}

	private async Task<WtqWindow?> FindWtqMainWindowAsync()
	{
		for (var i = 0; i < 10; i++)
		{
			var windows = await _windowService.GetWindowsAsync(CancellationToken.None).NoCtx();

			var mainWindow = windows.FirstOrDefault(w => w.Title == MainWindowTitle);

			if (mainWindow != null)
			{
				return mainWindow;
			}

			await Task.Delay(TimeSpan.FromMilliseconds(200)).NoCtx();
		}

		return null;
	}
}