using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WindowsTerminalQuake.Native;

namespace WindowsTerminalQuake.Settings
{
	public class SettingsDto
	{
		public static readonly List<Hotkey> DefaultHotkeys = new List<Hotkey>()
		{
			new Hotkey() { Key = Keys.Oemtilde, Modifiers = KeyModifiers.Control },
		};

		/// <summary>
		/// <para>Whether to keep the terminal window always on top (requires restart).</para>
		/// <para>Defaults to "false".</para>
		/// </summary>
		public bool AlwaysOnTop { get; set; } = false;

		/// <summary>
		/// <para>Disables toggling of the terminal window if the currently active application is running in fullscreen mode on primary monitor.</para>
		/// <para>Defaults to "false".</para>
		/// </summary>
		public bool DisableWhenActiveAppIsInFullscreen { get; set; } = false;

		/// <summary>
		/// <para>When clicking or alt-tabbing away to another app, the terminal will automatically (and instantly) hide.</para>
		/// <para>Defaults to "true".</para>
		/// </summary>
		public bool HideOnFocusLost { get; set; } = true;

		/// <summary>
		/// <para>When "HorizontalScreenCoverage" is below 100, this setting determines where the terminal is place horizontally.</para>
		/// <para>"Center" (default), "Left" or "Right".</para>
		/// </summary>
		public HorizontalAlign HorizontalAlign { get; set; } = HorizontalAlign.Center;

		/// <summary>
		/// <para>Horizontal screen coverage, as a percentage.</para>
		/// <para>Defaults to "100".</para>
		/// </summary>
		public int HorizontalScreenCoverage { get; set; } = 100;

		/// <summary>
		/// <para>The keys that can be used to toggle the terminal.</para>
		/// <para>See "Hotkeys" for possible values.</para>
		/// </summary>
		public List<Hotkey> Hotkeys { get; set; }

		/// <summary>
		/// <para>Minimum level of events that are logged.</para>
		/// <para>"Verbose", "Debug", "Information", "Warning", "Error" (default), "Fatal".</para>
		/// </summary>
		public LogEventLevel LogLevel { get; set; } = LogEventLevel.Error;

		/// <summary>
		/// <para>Whether to maximize the terminal after it has toggled into view.</para>
		/// <para>Note that this only applies when both <see cref="HorizontalScreenCoverage"/> and <see cref="VerticalScreenCoverage"/> are at least 100.</para>
		/// <para>Defaults to "true".</para>
		/// </summary>
		public bool MaximizeAfterToggle { get; set; } = true;

		/// <summary>
		/// <para>If "PreferMonitor" is set to "AtIndex", this setting determines what monitor to choose.</para>
		/// <para>Zero based, eg. 0, 1, etc.</para>
		/// <para>Defaults to "0".</para>
		/// </summary>
		public int MonitorIndex { get; set; }

		/// <summary>
		/// <para>Whether to show notifications when the app starts and when the settings are reloaded.</para>
		/// <para>Defaults to "true".</para>
		/// </summary>
		public bool Notifications { get; set; } = true;

		/// <summary>
		/// <para>Make the window see-through (applies to the entire window, including the title bar).</para>
		/// <para>0 (invisible) - 100 (opaque).</para>.
		/// <para>Defaults to "80".</para>
		/// </summary>
		public int Opacity { get; set; } = 80;

		/// <summary>
		/// <para>What monitor to preferrably drop the terminal.</para>
		/// <para>"WithCursor" (default), "Primary" or "AtIndex".</para>
		/// </summary>
		public PreferMonitor PreferMonitor { get; set; } = PreferMonitor.WithCursor;

		/// <summary>
		/// <para>Whether to hide the terminal window immediately after app start.</para>
		/// <para>Defaults to "false".</para>
		/// </summary>
		public bool StartHidden { get; set; } = false;

		/// <summary>
		/// <para>Temporarily disable the toggle hotkeys when any of these processes has focus.</para>
		/// <para>Defaults to no processes.</para>
		/// </summary>
		public List<string> SuppressHotkeyForProcesses { get; set; } = new List<string>();

		/// <summary>
		/// <para>When to show the terminal window icon on the taskbar.</para>
		/// <para>"AlwaysHidden", "AlwaysVisible" or "WhenTerminalVisible".</para>
		/// <para>Defaults to "AlwaysHidden".</para>
		/// </summary>
		public TaskBarIconVisibility TaskbarIconVisibility { get; set; } = TaskBarIconVisibility.AlwaysHidden;

		/// <summary>
		/// <para>Target time between animation frames.</para>
		///
		/// <para>The lower this is, the smoother the animation will be, but can also add a bit more load to the system.
		/// Especially when running on battery-powered laptops and the like, low frame times can prove problematic.</para>
		///
		/// <para>Since a change where the frame wait time is calculated dynamically, this doesn't have too much effect anymore.
		/// Perhaps we should remove the setting to avoid confusion.</para>
		/// </summary>
		public int ToggleAnimationFrameTimeMs { get; set; } = 25;

		/// <summary>
		/// <para>Which animation type is used during toggle up/down.</para>
		/// <para>"Linear", "EaseInBack", "EaseInCubic", "EaseInOutSine", "EaseInQuart", "EaseOutBack", "EaseOutCubic" or "EaseOutQuart" (default).</para>
		/// </summary>
		public AnimationType ToggleAnimationType { get; set; } = AnimationType.EaseOutQuart;

		/// <summary>
		/// <para>How long the toggle up/down takes in milliseconds.</para>
		/// <para>Defaults to "250".</para>
		/// </summary>
		public int ToggleDurationMs { get; set; } = 250;

		/// <summary>
		/// <para>How the terminal actually gets toggled on- and off the screen.</para>
		/// <para>"Resize" (default) or "Move".</para>
		/// </summary>
		public ToggleMode ToggleMode { get; set; } = ToggleMode.Resize;

		/// <summary>
		/// <para>How much room to leave between the top of the terminal and the top of the screen.</para>
		/// <para>Defaults to "0".</para>
		/// </summary>
		public int VerticalOffset { get; set; } = 0;

		/// <summary>
		/// <para>Vertical screen coverage as a percentage (0-100).</para>
		/// <para>Defaults to "100".</para>
		/// </summary>
		public int VerticalScreenCoverage { get; set; } = 100;

		/// <summary>
		/// <para>The command/file path to execute when the app starts and Windows Terminal is not yet running.</para>
		/// <para>Defaults to "wt.exe".</para>
		/// </summary>
		public string WindowsTerminalCommand { get; set; } = "wt.exe";

		/// <summary>
		/// Horizontal screen coverage as an index (0 - 1).
		/// </summary>
		internal float HorizontalScreenCoverageIndex => HorizontalScreenCoverage / 100f;

		/// <summary>
		/// The location of the file where the current settings were loaded from.
		/// Can be null if these are defaults.
		/// </summary>
		internal string? PathToSettings { get; set; }

		/// <summary>
		/// Vertical screen coverage as an index (0 - 1).
		/// </summary>
		internal float VerticalScreenCoverageIndex => VerticalScreenCoverage / 100f;

		public static SettingsDto ParseFile(string pathToSettings)
		{
			if (string.IsNullOrWhiteSpace(pathToSettings)) throw new ArgumentNullException(nameof(pathToSettings));

			// Load the file from disk
			string? settingsJson;

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

			// Parse JSON contents
			try
			{
				return ParseJson(settingsJson);
			}
			catch (Exception ex)
			{
				Log.Error($"Error parsing settings file '{pathToSettings}':\n\n{ex.Message}");
				throw;
			}
		}

		public static SettingsDto ParseJson(string settingsJson)
		{
			var newSettings = JsonConvert.DeserializeObject<SettingsDto>(settingsJson)
				?? throw new Exception("Something went wrong while loading the settings file (deserialization returned null).")
			;

			newSettings.Hotkeys ??= DefaultHotkeys;

			return newSettings;
		}
	}
}