using Microsoft.Extensions.Hosting;
using NotificationIcon.NET;
using Wtq.Configuration;
using Wtq.Events;

namespace Wtq.Services.KWin;

public sealed class KWinTrayIconService
	: IAsyncDisposable, IHostedService
{
	private readonly IHostApplicationLifetime _lifetime;
	private readonly IWtqBus _bus;
	private readonly IWtqUIService _ui;

	private NotifyIcon? _icon;
	private Worker? _loop;

	public KWinTrayIconService(
		IHostApplicationLifetime lifetime,
		IWtqBus bus,
		IWtqUIService ui)
	{
		_lifetime = lifetime;
		_bus = Guard.Against.Null(bus);
		_ui = Guard.Against.Null(ui);

		new Thread(ShowStatusIcon).Start();
	}

	public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	public async ValueTask DisposeAsync()
	{
		_icon?.Dispose();
		await (_loop?.DisposeAsync() ?? ValueTask.CompletedTask).NoCtx();
	}

	private void ShowStatusIcon()
	{
		var iconPath = WtqPaths.GetPathRelativeToWtqAppDir("assets", "icon-v2-64.png");

		_icon = NotifyIcon.Create(
			iconPath,
			[
				new MenuItem($"Version {WtqConstants.AppVersion}")
				{
					IsDisabled = true,
				},

				new SeparatorItem(),

				new MenuItem($"Open Project Website (GitHub)")
				{
					Click = (s, a) => Os.OpenUrl(WtqConstants.GitHubUrl),
				},

				new SeparatorItem(),

				new MenuItem("Open Main Window")
				{
					Click = (s, e) => _bus.Publish(new WtqUIRequestedEvent()),
				},

				new SeparatorItem(),

				new MenuItem("Open Settings File")
				{
					Click = (s, a) => Os.OpenFileOrDirectory(WtqOptionsPath.Instance.Path),
				},

				new MenuItem("Open Settings Directory")
				{
					Click = (s, a) => Os.OpenFileOrDirectory(Path.GetDirectoryName(WtqOptionsPath.Instance.Path)!),
				},

				new MenuItem("Open Logs")
				{
					Click = (s, a) => Os.OpenFileOrDirectory(WtqPaths.GetWtqLogDir()),
				},

				new SeparatorItem(),

				new MenuItem("Quit")
				{
					Click = (s, e) => _lifetime.StopApplication(),
				},
			]);

		_loop = new(
			nameof(KWinTrayIconService),
			ct =>
			{
				_ui.RunOnUIThread(() => _icon.MessageLoopIteration(true));

				return Task.CompletedTask;
			},
			TimeSpan.FromMilliseconds(200));
	}
}