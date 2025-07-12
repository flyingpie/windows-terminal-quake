namespace Wtq;

public static class WtqPaths
{
	private static string? _pathToAppDir;

	/// <summary>
	/// Path to app data:<br/>
	/// Windows:   C:/users/username/AppData/Roaming<br/>
	/// Linux:     /home/username/.config.
	/// </summary>
	public static string AppData => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

	/// <summary>
	/// Path to wtq app data:<br/>
	/// Windows:   C:/users/username/AppData/Roaming/wtq<br/>
	/// Linux:     /home/username/.config/wtq.
	/// </summary>
	public static string AppDataWtq => Path.Combine(AppData, "wtq");

	/// <summary>
	/// Path to user home dir:<br/>
	/// Windows:   C:/Users/username<br/>
	/// Linux:     /home/username.
	/// </summary>
	public static string UserHome => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

	/// <summary>
	/// When creating a WTQ configuration file at runtime, place it here.
	/// Windows:   C:/Users/username/.config/wtq/wtq.jsonc<br/>
	/// Linux:     /home/username/.config/wtq/wtq.jsonc.
	/// </summary>
	public static string PreferredWtqConfigPath => Path.Combine(AppDataWtq, "wtq.jsonc");

	/// <summary>
	/// Returns path to the WTQ config file, as specified by an environment variable.
	/// </summary>
	public static string? WtqConfigFromEnvVar => WtqEnv.ConfigFile;

	/// <summary>
	/// Ordered list of possible paths to WTQ configuration file.<br/>
	/// Files are checked front-to-back, first one that exists is used.
	/// </summary>
	public static IEnumerable<string> WtqConfigPaths { get; } =
	[
		WtqConfigFromEnvVar ?? string.Empty,

		// Next to wtq executable.
		Path.Combine(WtqAppDir, "wtq.json"),
		Path.Combine(WtqAppDir, "wtq.jsonc"),
		Path.Combine(WtqAppDir, "wtq.json5"),

		// In XDG config dir.
		Path.Combine(UserHome, ".config", "wtq.json"),
		Path.Combine(UserHome, ".config", "wtq.jsonc"),
		Path.Combine(UserHome, ".config", "wtq.json5"),

		// In user home dir.
		Path.Combine(UserHome, "wtq.json"),
		Path.Combine(UserHome, "wtq.jsonc"),
		Path.Combine(UserHome, "wtq.json5"),

		// In user home dir, as a dot file.
		Path.Combine(UserHome, ".wtq.json"),
		Path.Combine(UserHome, ".wtq.jsonc"),
		Path.Combine(UserHome, ".wtq.json5"),

		// App data dir.
		Path.Combine(AppDataWtq, "wtq.json"),
		Path.Combine(AppDataWtq, "wtq.jsonc"),
		Path.Combine(AppDataWtq, "wtq.json5"),
	];

	/// <summary>
	/// Path to directory that contains the WTQ executable.
	/// </summary>
	public static string WtqAppDir
	{
		get
		{
			if (_pathToAppDir == null)
			{
				_pathToAppDir = Path.GetDirectoryName(typeof(WtqPaths).Assembly.Location);

				if (string.IsNullOrWhiteSpace(_pathToAppDir))
				{
					throw new WtqException("Could not get path to app directory.");
				}
			}

			return _pathToAppDir;
		}
	}

	/// <summary>
	/// Path to where log files are stored.
	/// Windows:    C:/users/username/appdata/local/temp/wtq<br/>
	/// Linux:      /home/username/.local/state.
	/// </summary>
	public static string WtqLogDir => WtqTempDir;

	/// <summary>
	/// Path to cache dir, where we can write temporary stuff. Currently used for GUI state and KWin script.
	/// Windows:    C:/users/username/appdata/local/temp/wtq<br/>
	/// Linux:      /home/username/.local/state.
	/// </summary>
	public static string WtqTempDir
	{
		get
		{
			if (Os.IsWindows)
			{
				return Path.Combine(Path.GetTempPath(), "wtq").GetOrCreateDirectory();
			}

			return Path.Combine(Xdg.XDG_STATE_HOME.GetOrCreateDirectory(), "wtq").GetOrCreateDirectory();
		}
	}

	public static string GetPathRelativeToWtqAppDir(params string[] path) => Path.Combine(WtqAppDir, Path.Combine(path));
}