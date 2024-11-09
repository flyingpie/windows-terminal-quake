using NotificationIcon.NET;
using System.Runtime.InteropServices;

namespace Wtq.Services.KWin;

public sealed class KWinTrayIconService : IAsyncDisposable, IAsyncInitializable
{
	// NotifyIcon icon;
	Thread _iconThread;

	public async Task InitializeAsync()
	{
		// _iconThread = new Thread(() =>
		// {
			string iconPath = AppContext.BaseDirectory;
			iconPath = Path.Join(iconPath, "icon.ico");

			// if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			// {
			// }
			// else
			// {
			// 	iconPath = Path.Join(iconPath, "icon.png");
			// }

			var icon = NotifyIcon.Create(iconPath, new List<MenuItem>()
			{
				new MenuItem("Example Button"),
				new MenuItem("Example Checkbox")
				{
					IsChecked = true,
					Click = (s, e) =>
					{
						MenuItem me = (MenuItem)s!;
						me.IsChecked = !me.IsChecked;
					}
				},
				new MenuItem("Quit")
				{
					Click = (s, e) =>
					{
						e.Icon.Dispose();
					}
				}
			});

			var dbg2 =2;

			icon.Show();

			var dbg =2;
		// });

		// _iconThread.SetApartmentState(ApartmentState.STA);
		// _iconThread.Start();
	}

	public async ValueTask DisposeAsync()
	{
		// TODO release managed resources here
		var dbg = 2;
	}
}