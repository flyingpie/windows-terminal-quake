using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WindowsTerminalQuake.Native
{
	public class FocusTracker
	{
		public static event EventHandler OnFocusLost = delegate { };

		private static bool _isRunning;

		public static void FocusGained(Process process)
		{
			Log.Information("Focus gained");

			if (_isRunning) return;

			_isRunning = true;

			Task.Run(async () =>
			{
				while (_isRunning)
				{
					await Task.Delay(TimeSpan.FromMilliseconds(250));

					var main = process.MainWindowHandle;
					if (main != IntPtr.Zero)
					{
						var fg = User32.GetForegroundWindow();
						if (process.MainWindowHandle != fg)
						{
							Log.Information("Focus lost");
							OnFocusLost(null, null);
							_isRunning = false;
							break;
						}
					}
				}
			});
		}
	}
}