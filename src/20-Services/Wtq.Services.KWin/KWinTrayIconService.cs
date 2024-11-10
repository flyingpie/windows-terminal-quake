using NotificationIcon.NET;

namespace Wtq.Services.KWin;

public sealed class KWinTrayIconService
	: IAsyncDisposable, IAsyncInitializable
{
	private readonly IWtqBus _bus;
	private readonly IWtqUIThreadService _uiThread;

	private NotifyIcon _icon;

	// NotifyIcon icon;
	Thread _iconThread;

	public KWinTrayIconService(
		IWtqBus bus,
		IWtqUIThreadService uiThread)
	{
		_bus = bus;
		_uiThread = uiThread;

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
						_uiThread.OpenMainWindow();
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
			_uiThread.RunOnUIThread(() =>
			{
				_icon.MessageLoopIteration(true);
			});

			Thread.Sleep(100);
		}
	}
}