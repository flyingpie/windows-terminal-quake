using WindowsTerminalQuake.Utils;

namespace WindowsTerminalQuake.ScreenBoundsProviders;

public class ScreenBoundsProvider : IScreenBoundsProvider
{
	private readonly ILogger _log = Log.For<ScreenBoundsProvider>();

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
				_log.LogInformation($"Selecting screen at index '{settings.MonitorIndex}'.");

				// Make sure the monitor index is within bounds
				if (settings.MonitorIndex < 0)
				{
					_log.LogWarning($"Setting '{nameof(QSettings.Instance.MonitorIndex)}' must be greater than or equal to 0.");
					return Screen.PrimaryScreen.Bounds;
				}

				if (settings.MonitorIndex >= scr.Length)
				{
					_log.LogWarning($"Setting '{nameof(QSettings.Instance.MonitorIndex)}' ({settings.MonitorIndex}) must be less than the monitor count ({scr.Length}).");
					return Screen.PrimaryScreen.Bounds;
				}

				return scr[settings.MonitorIndex].Bounds;

			// Primary
			case PreferMonitor.Primary:
				_log.LogInformation($"Selecting primary screen.");

				return Screen.PrimaryScreen.Bounds;

			// With Cursor
			default:
			case PreferMonitor.WithCursor:
				_log.LogInformation($"Selecting screen with cursor.");

				return Screen.AllScreens
					.FirstOrDefault(s => s.Bounds.Contains(Cursor.Position))
					?.Bounds
					?? Screen.PrimaryScreen.Bounds
				;
		}
	}
}