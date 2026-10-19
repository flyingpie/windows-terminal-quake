using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationIcon.NET;

namespace Wtq.Services.TrayIcon;

public sealed class WtqTrayIconService : BackgroundService
{
	private readonly ILogger _log = Log.For<WtqTrayIconService>();

	private readonly IPlatformService _platform;
	private readonly TrayIconUtil _trayIconUtil;
	private readonly NotifyIcon _icon;

	public WtqTrayIconService(
		IHostApplicationLifetime lifetime,
		IPlatformService platform,
		TrayIconUtil trayIconUtil
	)
	{
		_ = Guard.Against.Null(lifetime);
		_platform = Guard.Against.Null(platform);
		_trayIconUtil = trayIconUtil;

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
				CreateItem(
					"Open Main Window",
					() => {
						/* TODO */
					}
				),
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
				CreateItem("Quit", lifetime.StopApplication),
			]
		);
	}

	private static MenuItem CreateItem(string text, Action action, bool enabled = true) =>
		new(text) { Click = (_, _) => action(), IsDisabled = !enabled };

	public override Task StartAsync(CancellationToken cancellationToken)
	{
		new Thread(ShowStatusIcon).Start();

		return Task.CompletedTask;
	}

	public override void Dispose()
	{
		_icon.Dispose();
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
		}
	}

	private void ShowStatusIcon()
	{
		_icon.Show();
	}
}
