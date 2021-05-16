using Serilog;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsTerminalQuake.Settings;

namespace WindowsTerminalQuake
{
	public interface IScreenBoundsProvider
	{
		/// <summary>
		/// Returns a bounding box for the screen where the terminal should be positioned on.
		/// </summary>
		Rectangle GetTargetScreenBounds();
	}

	public class ScreenBoundsProvider : IScreenBoundsProvider
	{
		/// <inheritdoc/>

		public Rectangle GetTargetScreenBounds()
		{
			var settings = QSettings.Instance;
			if (settings == null) return Screen.PrimaryScreen.Bounds; // Should not happen

			var scr = Screen.AllScreens;

			switch (settings.PreferMonitor)
			{
				// At Index
				case PreferMonitor.AtIndex:
					Log.Information($"Selecting screen at index '{settings.MonitorIndex}'.");

					// Make sure the monitor index is within bounds
					if (settings.MonitorIndex < 0)
					{
						Log.Warning($"Setting '{nameof(QSettings.Instance.MonitorIndex)}' must be greater than or equal to 0.");
						return Screen.PrimaryScreen.Bounds;
					}

					if (settings.MonitorIndex >= scr.Length)
					{
						Log.Warning($"Setting '{nameof(QSettings.Instance.MonitorIndex)}' ({settings.MonitorIndex}) must be less than the monitor count ({scr.Length}).");
						return Screen.PrimaryScreen.Bounds;
					}

					return scr[settings.MonitorIndex].Bounds;

				// Primary
				case PreferMonitor.Primary:
					Log.Information($"Selecting primary screen.");

					return Screen.PrimaryScreen.Bounds;

				// With Cursor
				default:
				case PreferMonitor.WithCursor:
					Log.Information($"Selecting screen with cursor.");

					return Screen.AllScreens
						.FirstOrDefault(s => s.Bounds.Contains(Cursor.Position))
						?.Bounds
						?? Screen.PrimaryScreen.Bounds
					;
			}
		}
	}
}