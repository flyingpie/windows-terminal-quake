using Newtonsoft.Json;
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

namespace WindowsTerminalQuake.Settings
{
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

	public class Hotkey
	{
		public Keys Key { get; set; }
		public KeyModifiers Modifiers { get; set; }
	}

	public class QSettings
	{
		public static readonly string[] PathsToSettings = new[]
		{
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), SettingsFile),
			Path.Combine(Path.GetDirectoryName(new Uri(typeof(QSettings).Assembly.Location).LocalPath), SettingsFile),
		};

		public static readonly string SettingsFile = "windows-terminal-quake.json";
		private static readonly List<Action<SettingsDto>> _listeners = new List<Action<SettingsDto>>();
		public static SettingsDto Instance { get; private set; } = new SettingsDto();

		public static void Get(Action<SettingsDto> action)
		{
			_listeners.Add(action);

			action(Instance);
		}

		#region Loading & Reloading

		private static readonly List<FileSystemWatcher> _fsWatchers;

		private static readonly RetryPolicy Retry = Policy
			.Handle<IOException>()
			.WaitAndRetry(new[] {
				TimeSpan.FromMilliseconds(250),
				TimeSpan.FromSeconds(1),
				TimeSpan.FromSeconds(1)
			})
		;

		static QSettings()
		{
			_fsWatchers = SetupWatchers();

			Reload(false);
		}

		public static void Reload(bool notify)
		{
			try
			{
				// We're doing the reload in a retry delegate, since sometimes the file is not readable due to other processes still using it
				// (eg. an editor that _just_ saved the file and hasn't released it yet).
				Retry.Execute(() =>
				{
					Log.Information("Reloading settings");

					foreach (var pathToSettings in PathsToSettings)
					{
						// Make sure that the file exists
						if (!File.Exists(pathToSettings))
						{
							// Note that the file isn't mandatory, so we're not gonna make too much of a fuss about that, other than logging a warning.
							Log.Warning($"Settings file at '{pathToSettings}' does not exist");
							continue;
						}

						Log.Information($"Found settings file at '{pathToSettings}'");

						// Load the file from disk
						string? settingsJson = null;
						try
						{
							settingsJson = File.ReadAllText(pathToSettings);
						}
						catch (IOException ex)
						{
							Log.Error($"Could not load settings from file '{pathToSettings}' {ex.GetType().FullName}: {ex.Message}", ex);
							throw;
						}

						Log.Information($"Loaded settings from '{pathToSettings}'.");

						SettingsDto? newSettings = null;

						try
						{
							newSettings = SettingsDto.Parse(settingsJson);
						}
						catch (Exception ex)
						{
							MessageBox.Show($"Error parsing settings file '{pathToSettings}':\n\n{ex.Message}");
							break;
						}

						if (newSettings == null) throw new Exception($"Settings was null after parsing, this is probably a bug.");

						Log.Information($"Parsed settings  from '{pathToSettings}'.");

						Instance = newSettings;

						if (notify) TrayIcon.Instance.Notify(ToolTipIcon.Info, $"Reloaded settings.");
						break;
					}

					_listeners.ForEach(l => l(Instance));
				});
			}
			catch (Exception ex)
			{
				Log.Error(ex, ex.Message);
				MessageBox.Show($"Error (re)loading settings: ({ex.GetType().FullName}) {ex.Message}. See the log for more information.");
			}
		}

		public static List<FileSystemWatcher> SetupWatchers()
		{
			return PathsToSettings
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
		}

		#endregion Loading & Reloading
	}

	public class SettingsDto
	{
		/// <summary>
		/// Whether to keep the terminal window always on top (requires restart).
		/// </summary>
		public bool AlwaysOnTop { get; set; } = false;

		/// <summary>
		/// Disables toggling of the terminal window if the currently active application is running in fullscreen mode on primary monitor.
		/// </summary>
		public bool DisableWhenActiveAppIsInFullscreen { get; set; } = false;

		/// <summary>
		/// When clicking or alt-tabbing away to another app, the terminal will automatically (and instantly) hide.
		/// </summary>
		public bool HideOnFocusLost { get; set; } = true;

		/// <summary>
		/// When "HorizontalScreenCoverage" is below 100, this setting determines where the terminal is place horizontally.<br/>
		/// "Center", "Left" or "Right".
		/// </summary>
		public HorizontalAlign HorizontalAlign { get; set; } = HorizontalAlign.Center;

		/// <summary>
		/// Horizontal screen coverage, as a percentage.
		/// </summary>
		public int HorizontalScreenCoverage { get; set; } = 100;

		private List<Hotkey> _hotkeys = new List<Hotkey>()
		{
			new Hotkey() { Key = Keys.Oemtilde, Modifiers = KeyModifiers.Control },
		};

		/// <summary>
		/// The keys that can be used to toggle the terminal.<br/>
		/// See "HotKeys" bellow for possible values.
		/// </summary>
		public List<Hotkey> Hotkeys
		{
			get => _hotkeys;
			set { if (value != null) _hotkeys = value; }
		}

		/// <summary>
		/// Minimum level of events that are logged.<br/>
		/// "Verbose", "Debug", "Information", "Warning", "Error", "Fatal".
		/// </summary>
		public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

		/// <summary>
		/// If "PreferMonitor" is set to "AtIndex", this setting determines what monitor to choose.<br/>
		/// Zero based, eg. 0, 1, etc.
		/// </summary>
		public int MonitorIndex { get; set; }

		/// <summary>
		/// Whether to show notifications when the app starts and when the settings are reloaded.
		/// </summary>
		public bool Notifications { get; set; } = true;

		/// <summary>
		/// Make the window see-through (applies to the entire window, including the title bar).<br/>
		/// 0 (invisible) - 100 (opaque)
		/// </summary>
		public int Opacity { get; set; } = 80;

		/// <summary>
		/// What monitor to preferrably drop the terminal.<br/>
		/// "WithCursor" (default), "Primary" or "AtIndex"
		/// </summary>
		public PreferMonitor PreferMonitor { get; set; } = PreferMonitor.WithCursor;

		/// <summary>
		/// Whether to hide the terminal window immediately after app start.
		/// </summary>
		public bool StartHidden { get; set; } = false;

		/// <summary>
		/// Target time between animation frames.
		///
		/// The lower this is, the smoother the animation will be, but can also add a bit more load to the system.
		/// Especially when running on battery-powered laptops and the like, low frame times can prove problematic.
		///
		/// Since a change where the frame wait time is calculated dynamically, this doesn't have too much effect anymore.
		/// Perhaps we should remove the setting to avoid confusion.
		/// </summary>
		public int ToggleAnimationFrameTimeMs { get; set; } = 25;

		/// <summary>
		/// Which animation type is used during toggle up/down.<br/>
		/// "Linear", "EaseInBack", "EaseInCubic", "EaseInOutSine", "EaseInQuart", "EaseOutBack", "EaseOutCubic" or "EaseOutQuart".
		/// </summary>
		public AnimationType ToggleAnimationType { get; set; } = AnimationType.Linear;

		/// <summary>
		/// How long the toggle up/down takes in milliseconds.
		/// </summary>
		public int ToggleDurationMs { get; set; } = 250;

		/// <summary>
		/// How much room to leave between the top of the terminal and the top of the screen.
		/// </summary>
		public int VerticalOffset { get; set; } = 0;

		/// <summary>
		/// Vertical screen coverage as a percentage (0-100).
		/// </summary>
		public int VerticalScreenCoverage { get; set; } = 100;

		/// <summary>
		/// Horizontal screen coverage as an index (0 - 1).
		/// </summary>
		internal float HorizontalScreenCoverageIndex => HorizontalScreenCoverage / 100f;

		/// <summary>
		/// Vertical screen coverage as an index (0 - 1).
		/// </summary>
		internal float VerticalScreenCoverageIndex => VerticalScreenCoverage / 100f;

		public static SettingsDto Parse(string settingsJson)
		{
			var newSettings = JsonConvert.DeserializeObject<SettingsDto>(settingsJson)
				?? throw new Exception("Something went wrong while loading the settings file (deserialization returned null).")
			;

			return newSettings;
		}
	}
}