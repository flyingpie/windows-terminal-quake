using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photino.Blazor;
using Wtq.Configuration;

namespace Wtq.Services.UI;

public class WtqUIHost
{
	private const string MainWindowTitle = "WTQ - Main Window";

	private readonly ILogger _log = Log.For<WtqUIHost>();

	private readonly IOptions<WtqOptions> _opts;
	private readonly IWtqWindowService _windowService;
	private readonly PhotinoBlazorApp _app;

	private bool _isClosing;

	public WtqUIHost(
		IOptions<WtqOptions> opts,
		IWtqBus bus,
		IWtqWindowService windowService,
		PhotinoBlazorApp app)
	{
		_app = Guard.Against.Null(app);
		_opts = Guard.Against.Null(opts);
		_windowService = Guard.Against.Null(windowService);

		bus.OnEvent<WtqUIRequestedEvent>(_ => OpenMainWindowAsync());

		SetupMainWindow();
	}

	private void SetupMainWindow()
	{
		_ = _app.MainWindow
			.RegisterWindowCreatedHandler((s, a) =>
			{
				if (!_opts.Value.GetShowUiOnStart())
				{
					_ = Task.Run(CloseMainWindowAsync);
				}
			})
			.RegisterWindowClosingHandler((s, a) =>
			{
				if (_isClosing)
				{
					return false;
				}

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

	/// <summary>
	/// Close UI (like, for real, as opposed to just hiding it).
	/// </summary>
	public void Exit()
	{
		_isClosing = true;
		_app.MainWindow.Close();
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