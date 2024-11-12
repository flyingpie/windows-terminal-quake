using NotificationIcon.NET;

namespace Wtq.Services.KWin;

public sealed class KWinTrayIconService
	: IAsyncDisposable, IAsyncInitializable
{
	private readonly IWtqBus _bus;
	private readonly IWtqUIService _ui;

	private Thread? _iconThread;
	private NotifyIcon? _icon;

	public KWinTrayIconService(
		IWtqBus bus,
		IWtqUIService ui)
	{
		_bus = bus;
		_ui = ui;

		_iconThread = new Thread(ShowStatusIcon);
		_iconThread.Start();
	}

	public Task InitializeAsync()
	{
		return Task.CompletedTask;
	}

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}

	private void ShowStatusIcon()
	{
		var iconPath = Path.Join(AppContext.BaseDirectory, "icon.png");

		_icon = NotifyIcon.Create(
			iconPath,
			[
				new MenuItem("Open Settings")
				{
					Click = (s, e) => { _ = Task.Run(() => _ui.OpenMainWindowAsync()); },
				},
				new MenuItem("Quit")
				{
					Click = (s, e) => { e.Icon.Dispose(); },
				},
			]);

		while (true)
		{
			_ui.RunOnUIThread(() => { _icon.MessageLoopIteration(true); });

			Thread.Sleep(100);
		}
	}
}