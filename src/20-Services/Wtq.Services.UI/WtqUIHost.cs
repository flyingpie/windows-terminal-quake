using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photino.Blazor;
using Wtq.Configuration;

namespace Wtq.Services.UI;

public class WtqUIHost
{
	private const string MainWindowTitle = "WTQ - Main Window";

	private readonly ILogger _log = Log.For<WtqUIHost>();
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

				Log.CloseAndFlush();

				_ = Task.Run(async () =>
				{
					await Task.Delay(TimeSpan.FromSeconds(2));

					Environment.Exit(0);
				});
			});

		_ = app.MainWindow
			.RegisterWindowCreatedHandler((s, a) =>
			{
				if (!opts.Value.GetShowUiOnStart())
				{
					_ = Task.Run(CloseMainWindowAsync);
				}
			})
			.RegisterWindowClosingHandler((s, a) =>
			{
				_ = Task.Run(CloseMainWindowAsync);

				return !_isClosing;
			})
			.Center()
			.SetIconFile(WtqPaths.GetPathRelativeToWtqAppDir("assets", "icon-v2-256-padding.png"))
			.SetLogVerbosity(0)
			.SetSize(1280, 800)
			.SetTitle(MainWindowTitle);
	}

	private async Task CloseMainWindowAsync()
	{
		_app.MainWindow.SetMinimized(true);

		var w = await FindWtqMainWindowAsync().NoCtx();

		if (w == null)
		{
			_log.LogWarning("Could not find WTQ main window");
			return;
		}

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

			var mainWindow = windows.FirstOrDefault(w => w.WindowTitle == MainWindowTitle);

			if (mainWindow != null)
			{
				return mainWindow;
			}

			await Task.Delay(TimeSpan.FromMilliseconds(200)).NoCtx();
		}

		return null;
	}
}