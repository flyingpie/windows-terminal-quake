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
		/// The location of the file where the current settings were loaded from.
		/// Can be null if these are defaults.
		/// </summary>
		public string? PathToSettings { get; set; }

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

		// TODO: Make some tests that these are correctly defaulted/overridden.

		/// <summary>
		/// The keys that can be used to toggle the terminal.<br/>
		/// See "HotKeys" bellow for possible values.
		/// </summary>
		public List<Hotkey> Hotkeys { get; set; }

		/// <summary>
		/// Minimum level of events that are logged.<br/>
		/// "Verbose", "Debug", "Information", "Warning", "Error", "Fatal".
		/// </summary>
		public LogEventLevel LogLevel { get; set; } = LogEventLevel.Error;

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
				return SettingsDto.ParseJson(settingsJson);
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