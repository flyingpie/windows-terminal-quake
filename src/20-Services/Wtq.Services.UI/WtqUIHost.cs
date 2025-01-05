using Microsoft.Extensions.Hosting;
using Photino.Blazor;
using System.Collections.Generic;
using System.Drawing;

namespace Wtq.Services.UI;

public class WtqUIHost
{
	private const string MainWindowTitle = "WTQ - Main Window";
	private static readonly Point OffScreenLocation = new(0, -1_000_000);

	private readonly IWtqWindowService _windowService;
	private readonly IWtqScreenInfoProvider _screenInfoProvider;
	private bool _isClosing;

	public WtqUIHost(
		IEnumerable<IHostedService> hostedServices,
		IHostApplicationLifetime appLifetime,
		IWtqBus bus,
		IWtqScreenInfoProvider screenInfoProvider,
		IWtqWindowService windowService,
		PhotinoBlazorApp app)
	{
		_screenInfoProvider = Guard.Against.Null(screenInfoProvider);
		_windowService = Guard.Against.Null(windowService);

		_ = Guard.Against.Null(app);
		_ = Guard.Against.Null(appLifetime);
		_ = Guard.Against.Null(bus);
		_ = Guard.Against.Null(hostedServices);

		bus.OnEvent<WtqUIRequestedEvent>(e => OpenMainWindowAsync());

		_ = appLifetime.ApplicationStarted.Register(() =>
		{
			Task
				.Run(async () =>
				{
					foreach (var srv in hostedServices)
					{
						await srv.StartAsync(CancellationToken.None).NoCtx();
					}
				})
				.GetAwaiter()
				.GetResult();
		});

		_ = appLifetime.ApplicationStopping.Register(
			() =>
			{
				Task
					.Run(async () =>
					{
						foreach (var srv in hostedServices)
						{
							await srv.StopAsync(CancellationToken.None).NoCtx();
						}
					})
					.GetAwaiter()
					.GetResult();

				_isClosing = true;

				app.MainWindow.Close();

				Task
					.Run(async () =>
					{
						foreach (var srv in hostedServices.OfType<IAsyncDisposable>())
						{
							await srv.DisposeAsync().NoCtx();
						}
					})
					.GetAwaiter()
					.GetResult();
			});

		_ = app.MainWindow
			.RegisterWindowCreatedHandler((s, a) =>
			{
				_ = Task.Run(CloseMainWindowAsync);
			})
			.RegisterWindowClosingHandler((s, a) =>
			{
				_ = Task.Run(CloseMainWindowAsync);

				return !_isClosing;
			})
			.SetIconFile(WtqPaths.GetPathRelativeToWtqAppDir("assets", "icon-v2-256-padding.png"))
			.SetLogVerbosity(0)
			.SetTitle(MainWindowTitle);
	}

	private async Task CloseMainWindowAsync()
	{
		var w = await FindWtqMainWindowAsync().NoCtx();

		if (w == null)
		{
			return;
		}

		await w.MoveToAsync(OffScreenLocation).NoCtx();
		await w.SetTaskbarIconVisibleAsync(false).NoCtx();
	}

	private async Task OpenMainWindowAsync()
	{
		var w = await FindWtqMainWindowAsync().NoCtx();

		if (w == null)
		{
			return;
		}

		var scrRect = await _screenInfoProvider.GetScreenWithCursorAsync().NoCtx();
		var wndRect = await w.GetWindowRectAsync().NoCtx();

		var loc = new Point(
			x: scrRect.X + (scrRect.Width / 2) - (wndRect.Width / 2),
			y: scrRect.Y + (scrRect.Height / 2) - (wndRect.Height / 2));

		await w.MoveToAsync(loc).NoCtx();
		await w.BringToForegroundAsync().NoCtx();
		await w.SetTaskbarIconVisibleAsync(true).NoCtx();
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