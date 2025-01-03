using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photino.Blazor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Wtq.Configuration;

namespace Wtq.Services.UI;

public sealed class WtqUI
{


	// private readonly ILogger _log = Log.For<WtqUI>();
	// private readonly IHostApplicationLifetime _appLifetime;
	private readonly IWtqWindowService _windowService;

	// private PhotinoBlazorApp? _app;
	private Point? _loc;
	// private bool _isClosing;

	public WtqUI(
		// IHostApplicationLifetime appLifetime,
		// IWtqBus bus,
		IWtqWindowService windowService)
	{
		// _appLifetime = Guard.Against.Null(appLifetime);
		// _ = Guard.Against.Null(bus);
		_windowService = Guard.Against.Null(windowService);
	}

	// public void Init(PhotinoBlazorApp app)
	// {
	// 	_app = app;
	//
	// 	_app.MainWindow.RegisterWindowClosingHandler(
	// 		(s, a) =>
	// 		{
	// 			_ = Task.Run(CloseMainWindowAsync);
	//
	// 			return !_isClosing;
	// 		});
	// }

	// protected override Task OnStartAsync(CancellationToken cancellationToken)
	// {
	// 	return Task.CompletedTask;
	// }

	// protected override ValueTask OnDisposeAsync()
	// {
	// 	// _uiThread?.Join();
	// 	//
	// 	// // On Linux, for some reason, the app lingers a bit when closing.
	// 	// // This seems to be a native thing, possibly around GTK.
	// 	// // Doesn't happen when the UI is disabled, doesn't happen on Windows, and I can't pause the process, suggesting that .Net is already done.
	// 	// // Workaround for now, to kill the entire process.
	// 	// if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
	// 	// {
	// 	// 	Process.GetCurrentProcess().Kill();
	// 	// }
	// 	//
	// 	return ValueTask.CompletedTask;
	// }

	// public async Task CloseMainWindowAsync()
	// {
	// 	var w = await FindWtqMainWindowAsync().NoCtx();
	//
	// 	if (w == null)
	// 	{
	// 		return;
	// 	}
	//
	// 	await w.MoveToAsync(new Point(0, -1_000_000)).NoCtx();
	// 	await w.SetTaskbarIconVisibleAsync(false).NoCtx();
	// }

	// public async Task OpenMainWindowAsync()
	// {
	// 	var w = await FindWtqMainWindowAsync().NoCtx();
	//
	// 	if (w == null)
	// 	{
	// 		return;
	// 	}
	//
	// 	await w.MoveToAsync(_loc ?? Point.Empty).NoCtx();
	// 	await w.BringToForegroundAsync().NoCtx();
	// 	await w.SetTaskbarIconVisibleAsync(true).NoCtx();
	// }

	// public void RunOnUIThread(Action action)
	// {
	// 	if (_isClosing)
	// 	{
	// 		_log.LogWarning("UI is stopping, skipping '{Name}' action", nameof(RunOnUIThread));
	// 		return;
	// 	}
	//
	// 	try
	// 	{
	// 		_app?.MainWindow?.Invoke(action);
	// 	}
	// 	catch (Exception ex)
	// 	{
	// 		_log.LogWarning(ex, "Error running action on UI thread: {Message}", ex.Message);
	// 	}
	// }



	// private async Task<WtqWindow?> FindWtqMainWindowAsync()
	// {
	// 	for (var i = 0; i < 10; i++)
	// 	{
	// 		var windows = await _windowService.GetWindowsAsync(CancellationToken.None).NoCtx();
	//
	// 		var mainWindow = windows.FirstOrDefault(w => w.Title == MainWindowTitle);
	//
	// 		if (mainWindow != null)
	// 		{
	// 			_loc ??= (await mainWindow.GetWindowRectAsync().NoCtx()).Location;
	// 			return mainWindow;
	// 		}
	//
	// 		await Task.Delay(TimeSpan.FromMilliseconds(200)).NoCtx();
	// 	}
	//
	// 	return null;
	// }

	public static void Run(Action<IServiceCollection> services)
	{
		var log = Log.For<WtqUI>();
		log.LogDebug("UI thread starting");

		var appBuilder = PhotinoBlazorAppBuilder.CreateDefault();

		var invoker = new WtqUIInvoker();

		// TODO: Unify with the main app DI.
		appBuilder.Services
			.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>()
			.AddSingleton<IWtqUIService>(_ => invoker)
			// .AddSingleton<WtqUI>()
			;

		services(appBuilder.Services);

		appBuilder.RootComponents.Add<App>("app");

		var app = appBuilder.Build();

		invoker.Action = a => app?.MainWindow?.Invoke(a);

		var _isClosing = false;

		// var ui = app.Services.GetRequiredService<WtqUI>();
		// var appLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
		// var bus = app.Services.GetRequiredService<IWtqBus>();

		var uiThing = new WtqWinExt(
			app.Services.GetRequiredService<IWtqBus>(),
			app.Services.GetRequiredService<IWtqWindowService>(),
			app.Services.GetRequiredService<IWtqScreenInfoProvider>(),
			app.Services.GetRequiredService<IHostApplicationLifetime>(),
			app.Services.GetRequiredService<IEnumerable<IHostedService>>(),
			app);

		((ApplicationLifetime)app.Services.GetRequiredService<IHostApplicationLifetime>()).NotifyStarted();

		app.Run();

		log.LogDebug("UI thread exiting");
	}
}

public class WtqUIInvoker : IDisposable, IWtqUIService
{
	private readonly ILogger _log = Log.For<WtqUIInvoker>();
	private bool _isClosing;

	public Action<Action> Action { get; set; }

	public void Dispose()
	{
		_isClosing = true;
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
			Action?.Invoke(action);
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error running action on UI thread: {Message}", ex.Message);
		}
	}
}

public class WtqWinExt
{
	private const string MainWindowTitle = "WTQ - Main Window";

	private readonly IWtqWindowService _windowService;
	private readonly IWtqScreenInfoProvider _scrInfoProvider;
	private readonly IEnumerable _hostedServices;
	private readonly PhotinoBlazorApp _app;
	private Point? _loc;
	private bool _isClosing;

	public WtqWinExt(
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

		await w.MoveToAsync(_loc ?? Point.Empty).NoCtx();
		await w.BringToForegroundAsync().NoCtx();
		await w.SetTaskbarIconVisibleAsync(true).NoCtx();
	}

	public async Task<WtqWindow?> FindWtqMainWindowAsync()
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
}