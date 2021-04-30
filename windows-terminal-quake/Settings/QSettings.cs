using Polly;
using Polly.Retry;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsTerminalQuake.UI;

namespace WindowsTerminalQuake.Settings
{
	public class QSettings
	{
		public const string SettingsFile = "windows-terminal-quake.json";

		public static readonly string[] PathsToSettings = new[]
		{
			// C:/path/to/windows-terminal-quake.json
			Path.Combine(Path.GetDirectoryName(new Uri(typeof(QSettings).Assembly.Location).LocalPath), SettingsFile),

			// C:/Users/username/windows-terminal-quake.json
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), SettingsFile),
		};

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
			.Handle<Exception>()
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

						var newSettings = SettingsDto.ParseFile(pathToSettings)
							?? throw new Exception($"Settings was null after parsing, this is probably a bug.")
						;

						newSettings.PathToSettings = pathToSettings;

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
}