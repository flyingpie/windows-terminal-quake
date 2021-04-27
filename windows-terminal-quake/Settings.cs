using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsTerminalQuake.Native;
using WindowsTerminalQuake.UI;

namespace WindowsTerminalQuake
{
	public class Settings
	{
		public static readonly string SettingsFile = "windows-terminal-quake.json";

		public static readonly string[] PathsToSettings = new[]
		{
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), SettingsFile),
			Path.Combine(Path.GetDirectoryName(new Uri(typeof(Settings).Assembly.Location).LocalPath), SettingsFile),
		};

		public static SettingsDto Instance { get; private set; } = new SettingsDto();

		private static readonly List<Action<SettingsDto>> _listeners = new List<Action<SettingsDto>>();

		public static void Get(Action<SettingsDto> action)
		{
			_listeners.Add(action);

			action(Instance);
		}

		#region Loading & Reloading

		private static readonly RetryPolicy Retry = Policy
			.Handle<Exception>()
			.WaitAndRetry(new[] {
				TimeSpan.FromMilliseconds(250),
				TimeSpan.FromSeconds(1),
				TimeSpan.FromSeconds(1)
			})
		;

		private static readonly List<FileSystemWatcher> _fsWatchers;

		static Settings()
		{
			_fsWatchers = PathsToSettings
				.Select(path =>
				{
					try
					{
						Log.Information($"Watching settings file '{path}' for changes");
						var fsWatcher = new FileSystemWatcher(Path.GetDirectoryName(path), Path.GetFileName(path));

						fsWatcher.Changed += (s, a) =>
						{
							Log.Information($"Settings file '{a.FullPath}' changed");
							Reload(true);
						};

						fsWatcher.EnableRaisingEvents = true;

						return fsWatcher;
					}
					catch (Exception ex)
					{
						Log.Error(ex, $"Could not load settings file at location '{path}': {ex.Message}");
						return null;
					}
				})
				.Where(s => s != null)
				.Select(s => s!)
				.ToList()
			;

			Application.ApplicationExit += (s, a) => _fsWatchers.ForEach(w => w.Dispose());

			Reload(false);
		}

		public static void Reload(bool notify)
		{
			try
			{
				Retry.Execute(() =>
				{
					Log.Information("Reloading settings");

					foreach (var pathToSettings in PathsToSettings)
					{
						if (!File.Exists(pathToSettings))
						{
							Log.Warning($"Settings file at '{pathToSettings}' does not exist");
							continue;
						}

						Log.Information($"Found settings file at '{pathToSettings}'");

						try
						{
							var settingsJson = File.ReadAllText(pathToSettings);

							Instance = JsonConvert
								.DeserializeObject<JObject>(settingsJson)
								.ToObject<SettingsDto>()
							;

							Log.Information($"Loaded settings from '{pathToSettings}'");
							if (notify) TrayIcon.Instance.Notify(ToolTipIcon.Info, $"Reloaded settings");
							break;
						}
						catch (Exception ex)
						{
							Log.Error($"Could not load settings from file '{pathToSettings}': {ex.Message}", ex);
							throw;
						}
					}

					_listeners.ForEach(l => l(Instance));
				});
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error (re)loading settings: {ex.Message}. See the log for more information.");
			}
		}

		#endregion Loading & Reloading
	}

	public class SettingsDto
	{
		public List<Hotkey> Hotkeys { get; set; }

		public bool Notifications { get; set; } = true;

		public int Opacity { get; set; } = 80;

		public int VerticalOffset { get; set; } = 0;

		/// <summary>
		/// Vertical screen coverage as a percentage (0-100).
		/// </summary>
		public float VerticalScreenCoverage { get; set; } = 100;

		/// <summary>
		/// Vertical screen coverage as an index (0 - 1).
		/// </summary>
		public float VerticalScreenCoverageIndex => VerticalScreenCoverage / 100f;

		public HorizontalAlign HorizontalAlign { get; set; } = HorizontalAlign.Center;

		/// <summary>
		/// Horizontal screen coverage, as a percentage.
		/// </summary>
		public float HorizontalScreenCoverage { get; set; } = 100;

		/// <summary>
		/// Horizontal screen coverage as an index (0 - 1).
		/// </summary>
		public float HorizontalScreenCoverageIndex => HorizontalScreenCoverage / 100f;

		public int ToggleDurationMs { get; set; } = 250;

		public int ToggleAnimationFrameTimeMs { get; set; } = 25;

		public AnimationType ToggleAnimationType { get; set; } = AnimationType.Linear;

		public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

		public bool HideOnFocusLost { get; set; } = true;

		public bool AlwaysOnTop { get; set; } = false;

		public bool StartHidden { get; set; } = false;

		public PreferMonitor PreferMonitor { get; set; } = PreferMonitor.WithCursor;

		public int MonitorIndex { get; set; }

		public bool DisableOnFullscreenWindow { get; set; } = true;
	}

	public class Hotkey
	{
		public KeyModifiers Modifiers { get; set; }

		public Keys Key { get; set; }
	}

	public enum AnimationType
	{
		Linear = 0,

		EaseInBack,
		EaseInCubic,
		EaseInOutSine,
		EaseInQuart,
		EaseOutBack,
		EaseOutCubic,
		EaseOutQuart,
	}

	public enum HorizontalAlign
	{
		Center = 0,

		Left,
		Right
	}

	public enum PreferMonitor
	{
		WithCursor = 0,

		AtIndex,
		Primary,
	}
}