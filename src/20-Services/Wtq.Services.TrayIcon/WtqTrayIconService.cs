using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationIcon.NET;

namespace Wtq.Services.TrayIcon;

public sealed class WtqTrayIconService(
	IHostApplicationLifetime lifetime,
	IPlatformService platform,
	TrayIconUtil trayIconUtil
) : BackgroundService
{
	private readonly ILogger _log = Log.For<WtqTrayIconService>();
	private readonly CancellationTokenSource _cts = new();

	private readonly IHostApplicationLifetime _lifetime = Guard.Against.Null(lifetime);
	private readonly IPlatformService _platform = Guard.Against.Null(platform);
	private readonly TrayIconUtil _trayIconUtil = Guard.Against.Null(trayIconUtil);

	private NotifyIcon? _icon;
	private Thread? _thread;

	private static MenuItem CreateItem(string text, Action action, bool enabled = true) =>
		new(text) { Click = (_, _) => action(), IsDisabled = !enabled };

	public override Task StartAsync(CancellationToken cancellationToken)
	{
		_thread = new Thread(ShowStatusIcon)
		{
			// Using a background thread to prevent the tray icon from delaying wtq exit.
			IsBackground = true,
			Name = nameof(WtqTrayIconService),
		};
		_thread.Start();

		return Task.CompletedTask;
	}

	public override void Dispose()
	{
		base.Dispose();

		_cts.Cancel();
		_cts.Dispose();
		_thread!.Join(TimeSpan.FromSeconds(1));
		_icon!.Dispose();
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
		await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken).NoThrowAwaitable();

	private void ShowStatusIcon()
	{
		_icon = NotifyIcon.Create(
			_trayIconUtil.TrayIconPath,
			[
				CreateItem(
					$"Version {WtqConstants.AppVersion} ({_platform.PlatformName})",
					() => { },
					enabled: false
				),
				new SeparatorItem(),
				CreateItem(
					"Open Project Website (GitHub)",
					() => _platform.OpenUrl(WtqConstants.GitHubUrl)
				),
				new SeparatorItem(),
				CreateItem("Open Main Window", _platform.OpenGui),
				new SeparatorItem(),
				CreateItem(
					"Open Settings File",
					() => _platform.OpenFileOrDirectory(_platform.PathToWtqConf)
				),
				CreateItem(
					"Open Settings Directory",
					() => _platform.OpenFileOrDirectory(_platform.PathToWtqConfDir)
				),
				CreateItem(
					"Open Logs",
					() => _platform.OpenFileOrDirectory(_platform.PathToLogsDir)
				),
				new SeparatorItem(),
				CreateItem("Quit", _lifetime.StopApplication),
			]
		);
		_icon.Show(_cts.Token);
	}
}
