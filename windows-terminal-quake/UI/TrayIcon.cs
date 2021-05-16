using Serilog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsTerminalQuake.Settings;

namespace WindowsTerminalQuake.UI
{
	public class TrayIcon : IDisposable
	{
		public static TrayIcon Instance { get; private set; }

		private NotifyIcon _notificationIcon;

		public TrayIcon(Action<object, EventArgs> exitHandler)
		{
			Instance = this;

			var waiter = new TaskCompletionSource<bool>();

			var notifyThread = new Thread(delegate ()
			{
				var contextMenu = new ContextMenu();

				contextMenu.MenuItems.AddRange(new[]
				{
					// Version
					CreateVersionItem(),

					// Open settings file
					CreateOpenSettingsItem(),

					// Exit
					CreateExitItem(exitHandler)
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

		private static MenuItem CreateVersionItem()
		{
			return new MenuItem($"Version v{Program.GetVersion()}")
			{
				Enabled = false
			};
		}

		private static MenuItem CreateOpenSettingsItem()
		{
			var mnuOpenSettings = new MenuItem("Open settings")
			{
				Enabled = true
			};

			mnuOpenSettings.Click += (s, a) =>
			{
				var path = QSettings.Instance.PathToSettings;

				if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
				{
					Log.Warning($"Settings file '{path}' not found, attempting to create an example file now and opening that.");

					//  Open the first default path.
					path = QSettings.PathsToSettings[0];

					// Make sure it doesn't already exist, and only then write some stub JSON.
					// "path" here may not be equal to "path" earlier, since we grabbed a path from PathToSettings.
					if (!File.Exists(path))
					{
						Log.Information($"Creating example file at '{path}'.");
						File.WriteAllText(path, _Resources.windows_terminal_quake);
					}
				}

				Process.Start(path);
			};

			return mnuOpenSettings;
		}

		private static MenuItem CreateExitItem(Action<object, EventArgs> exitHandler)
		{
			var mnuExit = new MenuItem("Exit");
			mnuExit.Click += new EventHandler(exitHandler);

			return mnuExit;
		}

		public void Dispose()
		{
			_notificationIcon?.Dispose();
			_notificationIcon = null;

			Application.Exit();
		}

		public void Notify(ToolTipIcon type, string message)
		{
			if (QSettings.Instance.Notifications)
			{
				_notificationIcon.ShowBalloonTip(3, $"Windows Terminal", message, type);
			}
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