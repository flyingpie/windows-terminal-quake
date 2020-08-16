using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsTerminalQuake.UI
{
	public class TrayIcon : IDisposable
	{
		private NotifyIcon _notificationIcon;

		public TrayIcon(Action<object, EventArgs> exitHandler)
		{
			var waiter = new TaskCompletionSource<bool>();

			var notifyThread = new Thread(delegate ()
			{
				var contextMenu = new ContextMenu();

				// Exit
				var mnuExit = new MenuItem("Exit");
				mnuExit.Click += new EventHandler(exitHandler);

				contextMenu.MenuItems.AddRange(new[]
				{
					mnuExit
				});

				// Tray Icon
				_notificationIcon = new NotifyIcon()
				{
					Icon = CreateIcon(),
					ContextMenu = contextMenu,
					Text = "Windows Terminal Quake",
					Visible = true
				};

				waiter.SetResult(true);

				Application.Run();
			});

			notifyThread.Start();

			waiter.Task.GetAwaiter().GetResult();
		}

		public void Dispose()
		{
			_notificationIcon?.Dispose();
			_notificationIcon = null;

			Application.Exit();
		}

		public void Notify(ToolTipIcon type, string message)
		{
			_notificationIcon.ShowBalloonTip(3, $"Windows Terminal", message, type);
		}

		private static Icon CreateIcon()
		{
			var bitmap = _Resources.icon.ToBitmap();
			bitmap.MakeTransparent(Color.White);
			var icH = bitmap.GetHicon();
			var ico = Icon.FromHandle(icH);
			return ico;
		}
	}
}