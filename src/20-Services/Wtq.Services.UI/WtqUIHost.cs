using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photino.Blazor;
using System.Diagnostics;
using Wtq.Configuration;

namespace Wtq.Services.UI;

public class WtqUIHost
{
	private const string MainWindowTitle = "WTQ - Main Window";

	private readonly ILogger _log = Log.For<WtqUIHost>();

	private readonly IOptions<WtqOptions> _opts;
	private readonly IEnumerable<IHostedService> _hostedServices;
	private readonly IHostApplicationLifetime _appLifetime;
	private readonly IWtqBus _bus;
	private readonly IWtqWindowService _windowService;
	private readonly PhotinoBlazorApp _app;

	private bool _isClosing;

	public WtqUIHost(
		IOptions<WtqOptions> opts,
		IEnumerable<IHostedService> hostedServices,
		IHostApplicationLifetime appLifetime,
		IWtqBus bus,
		IWtqWindowService windowService,
		PhotinoBlazorApp app)
	{
		_app = Guard.Against.Null(app);
		_appLifetime = Guard.Against.Null(appLifetime);
		_bus = bus = Guard.Against.Null(bus);
		_hostedServices = Guard.Against.Null(hostedServices);
		_windowService = Guard.Against.Null(windowService);
		_opts = Guard.Against.Null(opts);

		bus.OnEvent<WtqUIRequestedEvent>(_ => OpenMainWindowAsync());

		SetupAppLifetime();
		SetupMainWindow();
	}

	private void SetupAppLifetime()
	{
		// Starting
		_ = _appLifetime.ApplicationStarted.Register(() =>
		{
			_log.LogDebug("Stopping hosted services");

			var sw = Stopwatch.StartNew();

			Task.WaitAll(
				_hostedServices.Select(async srv =>
				{
					_log.LogDebug("Starting service '{Service}'", srv);
					await srv.StartAsync(CancellationToken.None);
					_log.LogDebug("Started service '{Service}', took {Elapsed}", srv, sw.Elapsed);
				}));
		});

		// Stopping
		_ = _appLifetime.ApplicationStopping.Register(() =>
		{
			Task.WaitAll(_hostedServices.Select(t => t.StopAsync(CancellationToken.None)));

			// // TODO: Remove this.
			// // When using SharpHook, some threads seem to hang around when otherwise exiting the app.
			// _ = Task.Run(async () =>
			// {
			// 	Console.WriteLine("EXIT");
			// 	await Task.Delay(TimeSpan.FromSeconds(2));
			//
			// 	Environment.Exit(0);
			// });

			((ApplicationLifetime)_appLifetime).NotifyStopped(); // "Stopped"
		});

		// Stopped
		_ = _appLifetime.ApplicationStopped.Register(() =>
		{
			Console.WriteLine("Waiting for hosted services to stop");
			Task.WaitAll(_hostedServices.OfType<IAsyncDisposable>().Select(t => t.DisposeAsync()).Select(t => t.AsTask()));
			Console.WriteLine("/Waiting for hosted services to stop");

			// Close UI (like, for real, as opposed to just hiding it).
			_isClosing = true;
			_app.MainWindow.Close();
		});
	}

	private void SetupMainWindow()
	{
		_ = _app.MainWindow
			.RegisterWindowCreatedHandler((s, a) =>
			{
				Console.WriteLine("RegisterWindowCreatedHandler");

				if (!_opts.Value.GetShowUiOnStart())
				{
					_ = Task.Run(CloseMainWindowAsync);
				}
			})
			.RegisterWindowClosingHandler((s, a) =>
			{
				Console.WriteLine("RegisterWindowClosingHandler");

				if (_isClosing)
				{
					Console.WriteLine("REALLY Closing");

					//					Thread.Sleep(5_000);
					return false;
				}

				Console.WriteLine("Not really Closing");
				_ = Task.Run(CloseMainWindowAsync);

				return true;
			})

			.Center()

			.SetIconFile(WtqPaths.GetPathRelativeToWtqAppDir("assets", "icon-v2-256-padding.png"))
			.SetJavascriptClipboardAccessEnabled(true)
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