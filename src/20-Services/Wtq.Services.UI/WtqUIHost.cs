using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Photino.Blazor;
using Wtq.Configuration;

namespace Wtq.Services.UI;

public class WtqUIHost
{
	private const string MainWindowTitle = "WTQ - Main Window";

	private readonly PhotinoBlazorApp _app;

	private readonly IWtqWindowService _windowService;
	private bool _isClosing;

	public WtqUIHost(
		IOptions<WtqOptions> opts,
		IEnumerable<IHostedService> hostedServices,
		IHostApplicationLifetime appLifetime,
		IWtqBus bus,
		IWtqWindowService windowService,
		PhotinoBlazorApp app)
	{
		_windowService = Guard.Against.Null(windowService);

		_app = Guard.Against.Null(app);
		_ = Guard.Against.Null(appLifetime);
		_ = Guard.Against.Null(bus);
		_ = Guard.Against.Null(hostedServices);

		bus.OnEvent<WtqUIRequestedEvent>(e => OpenMainWindowAsync());

		_ = appLifetime.ApplicationStarted.Register(() =>
		{
			Task.WaitAll(hostedServices.Select(srv => srv.StartAsync(CancellationToken.None)));
		});

		_ = appLifetime.ApplicationStopping.Register(
			() =>
			{
				Task.WaitAll(hostedServices.Select(t => t.StopAsync(CancellationToken.None)));

				_isClosing = true;

				app.MainWindow.Close();

				Task.WaitAll(hostedServices.OfType<IAsyncDisposable>().Select(t => t.DisposeAsync()).Select(t => t.AsTask()));
			});

		_ = app.MainWindow
			.RegisterWindowCreatedHandler((s, a) =>
			{
				if (!opts.Value.ShowUiOnStart)
				{
					_ = Task.Run(CloseMainWindowAsync);
				}
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

		_app.MainWindow.SetMinimized(true);

		await w.SetTaskbarIconVisibleAsync(false).NoCtx();
	}

	private async Task OpenMainWindowAsync()
	{
		var w = await FindWtqMainWindowAsync().NoCtx();

		if (w == null)
		{
			return;
		}

		_app.MainWindow.SetMinimized(false);

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