using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Photino.Blazor;
using System.Collections.Concurrent;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Exceptions;
using Wtq.Utils;
using Wtq.Utils.AsyncInit;

namespace Wtq.Services.UI;

public sealed class WtqUI : IAsyncInitializable
{
	private readonly IWtqWindowService _windowService;
	private readonly IWtqBus _bus;
	private readonly IWtqWindowService _processService;
	private readonly object _lock = new();

	private Thread? _uiThread;

	private PhotinoBlazorApp? _app;

	public WtqUI(
		IOptionsMonitor<WtqOptions> opts,
		IWtqBus bus,
		IWtqWindowService windowService,
		IWtqWindowService processService)
	{
		_windowService = windowService;
		_bus = Guard.Against.Null(bus);
		_processService = Guard.Against.Null(processService);

		// opts.OnChange(UpdateMainWindowVisibility);

		// UpdateMainWindowVisibility(opts.CurrentValue);

		_uiThread = new Thread(StartUI);

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			_uiThread.SetApartmentState(ApartmentState.STA);
		}

		_uiThread.Start();
	}

	public Task InitializeAsync()
	{
		_bus.OnEvent<WtqUIRequestedEvent>(
			e =>
			{
				// OpenMainWindow();

				return Task.CompletedTask;
			});

		_bus.OnEvent<WtqHotkeyPressedEvent>(
			async e =>
			{
				// if (_app == null)
				{
					// await OpenMainWindowAsync();
					await CloseMainWindowAsync();
				}

				// else
				// {
				// 	CloseMainWindow();
				// }
			});

		return Task.CompletedTask;
	}

	public async Task OpenMainWindowAsync()
	{
		Console.WriteLine("OpenMainWindow");

		var w = await FindWtqMainWindowAsync();

		if (w == null)
		{
			return;
		}

		await w.SetTaskbarIconVisibleAsync(false);
		await w.SetWindowVisibleAsync(false);
	}

	bool _isVisible = true;

	Point? _loc;

	public async Task CloseMainWindowAsync()
	{
		Console.WriteLine("CloseMainWindow");

		var w = await FindWtqMainWindowAsync();

		if (w == null)
		{
			return;
		}

		_loc ??= (await w.GetWindowRectAsync().NoCtx()).Location;

		_isVisible = !_isVisible;

		await w.SetTaskbarIconVisibleAsync(_isVisible);

		// await w.SetWindowVisibleAsync(_isVisible);

		if (_isVisible)
		{
			await w.MoveToAsync(_loc.Value);
			await w.BringToForegroundAsync().NoCtx();
		}
		else
		{
			await w.MoveToAsync(new Point(0, -1_000_000)).NoCtx();
		}

		// _app.MainWindow.SetLocation(new Point(0, -1_000_000));
	}

	public async Task<WtqWindow?> FindWtqMainWindowAsync()
	{
		for (int i = 0; i < 10; i++)
		{
			var windows = await _windowService.GetWindowsAsync();

			var mainWindow = windows.FirstOrDefault(w => w.Title == "WTQ - Main Window");

			if (mainWindow != null)
			{
				Console.WriteLine("Got WTQ main window");
				return mainWindow;
			}

			await Task.Delay(TimeSpan.FromMilliseconds(200));
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

			// .SetIconFile("wwwroot/img/icon.ico")
			.SetTitle("WTQ - Main Window");

		_app.MainWindow.RegisterWindowCreatedHandler(
			(s, a) =>
			{
				//
				Task.Run(async () => { await CloseMainWindowAsync(); });

				Console.WriteLine("CREATED");
			});

		_app.MainWindow.RegisterWindowClosingHandler(
			(s, a) =>
			{
				var dbg = 2;
				_app = null;

				Console.WriteLine();

				return false;
			});

		AppDomain.CurrentDomain.UnhandledException +=
			(sender, error) =>
			{
				//
				_app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
			};

		_app.Run();

		Console.WriteLine("CLOSE");
	}
}