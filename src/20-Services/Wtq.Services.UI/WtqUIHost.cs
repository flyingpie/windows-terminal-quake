using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Threading;
using Photino.Blazor;
using System.Diagnostics;
using Wtq.Configuration;
using IAsyncDisposable = System.IAsyncDisposable;

namespace Wtq.Services.UI;

public class WtqUIHost
{
	private const string MainWindowTitle = "WTQ - Main Window";

	private readonly ILogger _log = Log.For<WtqUIHost>();
	private readonly JoinableTaskFactory _taskFactory = new(JoinableTaskContext.CreateNoOpContext());

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
			//

			_taskFactory.Run(Services_StartAsync);
		});

		// Stopping
		_ = _appLifetime.ApplicationStopping.Register(() =>
		{
			//
			_taskFactory.Run(Services_StopAsync);

			((ApplicationLifetime)_appLifetime).NotifyStopped(); // "Stopped"
		});

		// Stopped (dispose)
		_ = _appLifetime.ApplicationStopped.Register(() =>
		{
			_taskFactory.Run(Services_DisposeAsync);

			// Close UI (like, for real, as opposed to just hiding it).
			_isClosing = true;
			_app.MainWindow.Close();
		});
	}

	private async Task Services_StartAsync()
	{
		var timeout = TimeSpan.FromSeconds(3);
		var sw1 = Stopwatch.StartNew();

		_log.LogDebug("Starting services");

		foreach (var srv in _hostedServices)
		{
			try
			{
				_log.LogDebug("Starting service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.StartAsync(new CancellationTokenSource(timeout).Token).TimeoutAfterAsync(timeout);
				_log.LogDebug("Started service '{Service}', took {Elapsed}", srv, sw2.Elapsed);
			}
			catch (Exception ex)
			{
				_log.LogCritical(ex, "Starting service '{Service}' failed", srv);
				Environment.Exit(-1);
				throw;
			}
		}

		_log.LogDebug("Started services, took {Elapsed}", sw1.Elapsed);
	}

	private async Task Services_StopAsync()
	{
		var sw1 = Stopwatch.StartNew();
		var timeout = TimeSpan.FromSeconds(3);

		_log.LogDebug("Stopping services");

		foreach (var srv in _hostedServices)
		{
			try
			{
				_log.LogDebug("Stopping service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.StopAsync(new CancellationTokenSource(timeout).Token).TimeoutAfterAsync(timeout);
				_log.LogDebug("Stopped service '{Service}', took {Elapsed}", srv, sw2.Elapsed);
			}
			catch (Exception ex)
			{
				_log.LogWarning(ex, "Stopping service '{Service}' failed", srv);
				Environment.Exit(-1);
			}
		}

		_log.LogDebug("Stopped services, took {Elapsed}", sw1.Elapsed);
	}

	private async Task Services_DisposeAsync()
	{
		var sw1 = Stopwatch.StartNew();
		var timeout = TimeSpan.FromSeconds(3);

		_log.LogDebug("Disposing services");

		foreach (var srv in _hostedServices.OfType<IAsyncDisposable>())
		{
			try
			{
				_log.LogDebug("Disposing service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.DisposeAsync().TimeoutAfterAsync(timeout);
				_log.LogDebug("Disposed service '{Service}', took {Elapsed}", srv, sw2.Elapsed);
			}
			catch (Exception ex)
			{
				_log.LogWarning(ex, "Disposing service '{Service}' failed", srv);
				Environment.Exit(-1);
			}
		}

		_log.LogDebug("Disposed services, took {Elapsed}", sw1.Elapsed);
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