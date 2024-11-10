using NotificationIcon.NET;

namespace Wtq.Services.KWin;

public sealed class KWinTrayIconService
	: IAsyncDisposable, IAsyncInitializable
{
	private readonly IWtqBus _bus;
	private readonly IWtqUIService _ui;

	private NotifyIcon _icon;

	// NotifyIcon icon;
	Thread _iconThread;

	public KWinTrayIconService(
		IWtqBus bus,
		IWtqUIService ui)
	{
		_bus = bus;
		_ui = ui;

		_iconThread = new Thread(ShowStatusIcon);
		_iconThread.Start();
	}

	public async Task InitializeAsync()
	{
	}

	public async ValueTask DisposeAsync()
	{
		// TODO release managed resources here
		var dbg = 2;
	}

	private void ShowStatusIcon()
	{
		string iconPath = AppContext.BaseDirectory;
		iconPath = Path.Join(iconPath, "icon.ico");

		_icon = NotifyIcon.Create(
			iconPath,
			new List<MenuItem>()
			{
				new MenuItem("Example Button")
				{
					Click = (s, e) =>
					{
						Console.WriteLine("Example Button!");
						// _bus.Publish(new WtqUIReadyEvent());
						_ = Task.Run(() => _ui.OpenMainWindowAsync());
					},
				},
				new MenuItem("Example Checkbox")
				{
					IsChecked = true,
					Click = (s, e) =>
					{
						MenuItem me = (MenuItem)s!;
						me.IsChecked = !me.IsChecked;
					},
				},
				new MenuItem("Quit")
				{
					Click = (s, e) => { e.Icon.Dispose(); }
				},
			});

		while (true)
		{
			_ui.RunOnUIThread(() =>
			{
				_icon.MessageLoopIteration(true);
			});

			Thread.Sleep(100);
		}
	}
}