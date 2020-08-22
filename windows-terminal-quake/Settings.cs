using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using Serilog;
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
		public static readonly string[] PathsToSettings = new[]
		{
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "windows-terminal-quake.json"),
			Path.Combine(Path.GetDirectoryName(new Uri(typeof(Settings).Assembly.Location).AbsolutePath), "windows-terminal-quake.json"),
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
			.WaitAndRetry(new[] { TimeSpan.FromMilliseconds(250), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1) })
		;

		private static readonly List<FileSystemWatcher> _fsWatchers;

		static Settings()
		{
			_fsWatchers = PathsToSettings
				.Select(path =>
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
				})
				.ToList()
			;

			Application.ApplicationExit += (s, a) => _fsWatchers.ForEach(w => w.Dispose());

			Reload(false);
		}

		public static void Reload(bool notify)
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
						if (notify) TrayIcon.Instance.Notify(ToolTipIcon.Info, $"Loaded settings from '{pathToSettings}'");
						break;
					}
					catch (Exception ex)
					{
						Log.Error($"Could not load settings from file '{pathToSettings}': {ex.Message}", ex);
					}
				}

				_listeners.ForEach(l => l(Instance));
			});
		}

		#endregion Loading & Reloading
	}

	public class SettingsDto
	{
		public List<Hotkey> Hotkeys { get; set; } = new List<Hotkey>()
		{
			new Hotkey() { Modifiers = KeyModifiers.Control, Key = Keys.Oemtilde },
			new Hotkey() { Modifiers = KeyModifiers.Control, Key = Keys.Q }
		};

		public bool Notifications { get; set; } = true;

		public int Opacity { get; set; } = 80;

		public float VerticalScreenCoverage { get; set; } = 100;

		public float HorizontalScreenCoverage { get; set; } = 100;

		public int ToggleDurationMs { get; set; } = 250;

		public bool Logging { get; set; } = false;
	}

	public class Hotkey
	{
		public KeyModifiers Modifiers { get; set; }

		public Keys Key { get; set; }
	}
}