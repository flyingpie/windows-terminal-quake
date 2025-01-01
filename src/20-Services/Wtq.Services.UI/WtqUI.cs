using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photino.Blazor;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Wtq.Configuration;

namespace Wtq.Services.UI;

public sealed class WtqUI : WtqHostedService, IWtqUIService
{
	private const string MainWindowTitle = "WTQ - Main Window";

	private readonly ILogger _log = Log.For<WtqUI>();
	private readonly IHostApplicationLifetime _appLifetime;
	private readonly IWtqWindowService _windowService;
	private readonly IWtqAppRepo _appRepo;
	private readonly IWtqScreenInfoProvider _screenInfoProvider;
	private readonly IServiceProvider _provider;
	private PhotinoBlazorApp? _app;
	private Point? _loc;
	private bool _isClosing;
	private Thread? _uiThread;

	public WtqUI(
		IHostApplicationLifetime appLifetime,
		IServiceProvider provider,
		IWtqAppRepo appRepo,
		IWtqBus bus,
		IWtqScreenInfoProvider screenInfoProvider,
		IWtqWindowService windowService)
	{
		_appLifetime = Guard.Against.Null(appLifetime);
		_appRepo = Guard.Against.Null(appRepo);
		_ = Guard.Against.Null(bus);
		_screenInfoProvider = Guard.Against.Null(screenInfoProvider);
		_windowService = Guard.Against.Null(windowService);

		bus.OnEvent<WtqUIRequestedEvent>(e => OpenMainWindowAsync());
		_provider = provider;
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		_uiThread = new Thread(StartUI);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			_uiThread.SetApartmentState(ApartmentState.STA);
		}

		_uiThread.Start();

		return Task.CompletedTask;
	}

	protected override ValueTask OnDisposeAsync()
	{
		_uiThread?.Join();

		// On Linux, for some reason, the app lingers a bit when closing.
		// This seems to be a native thing, possibly around GTK.
		// Doesn't happen when the UI is disabled, doesn't happen on Windows, and I can't pause the process, suggesting that .Net is already done.
		// Workaround for now, to kill the entire process.
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			Process.GetCurrentProcess().Kill();
		}

		return ValueTask.CompletedTask;
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
		if (_isClosing)
		{
			_log.LogWarning("UI is stopping, skipping '{Name}' action", nameof(RunOnUIThread));
			return;
		}

		try
		{
			_app?.MainWindow?.Invoke(action);
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error running action on UI thread: {Message}", ex.Message);
		}
	}

	private async Task<WtqWindow?> FindWtqMainWindowAsync()
	{
		for (var i = 0; i < 10; i++)
		{
			var windows = await _windowService.GetWindowsAsync(CancellationToken.None).NoCtx();

			var mainWindow = windows.FirstOrDefault(w => w.Title == MainWindowTitle);

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
		_log.LogDebug("UI thread starting");

		var appBuilder = PhotinoBlazorAppBuilder.CreateDefault();

		// TODO: Unify with the main app DI.
		appBuilder.Services
			.AddSingleton<IWtqAppRepo>(p => _appRepo)
			.AddSingleton<IWtqWindowService>(p => _windowService)
			.AddSingleton<IWtqScreenInfoProvider>(p => _screenInfoProvider)
			.AddTransient<IOptions<WtqOptions>>(p => _provider.GetRequiredService<IOptions<WtqOptions>>())
			.AddTransient<IOptionsMonitor<WtqOptions>>(p => _provider.GetRequiredService<IOptionsMonitor<WtqOptions>>())
			.AddSingleton<WtqOptionsSaveService>()
			.AddUI()
			.AddLogging();

		appBuilder.RootComponents.Add<App>("app");

		_app = appBuilder.Build();

		_app.MainWindow
			.SetLogVerbosity(0)
			.SetIconFile(WtqPaths.GetPathRelativeToWtqAppDir("assets", "icon-v2-64.png"))
			.SetTitle(MainWindowTitle);

		_app.MainWindow.RegisterWindowClosingHandler(
			(s, a) =>
			{
				_ = Task.Run(CloseMainWindowAsync);

				return !_isClosing;
			});

		_appLifetime.ApplicationStopping.Register(
			() =>
			{
				_isClosing = true;

				_app.MainWindow.Close();
			});

		_app.Run();

		_log.LogDebug("UI thread exiting");
	}
}