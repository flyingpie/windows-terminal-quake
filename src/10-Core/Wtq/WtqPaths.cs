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
	/// Linux:     /home/username/.config/wqt.
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
	/// Windows:   C:/Users/username/wtq.jsonc<br/>
	/// Linux:     /home/username/wtq.jsonc.
	/// </summary>
	// TODO: Put Linux version under "/home/username/.wtq.jsonc".
	public static string PreferredWtqConfigPath => Path.Combine(UserHome, "wtq.jsonc");

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
		Path.Combine(GetWtqAppDir(), "wtq.json"),
		Path.Combine(GetWtqAppDir(), "wtq.jsonc"),
		Path.Combine(GetWtqAppDir(), "wtq.json5"),

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

	public static string GetPathRelativeToWtqAppDir(params string[] path) => Path.Combine(GetWtqAppDir(), Path.Combine(path));

	/// <summary>
	/// Path to directory that contains the WTQ executable.
	/// </summary>
	public static string GetWtqAppDir()
	{
		if (_pathToAppDir == null)
		{
			_pathToAppDir = Path.GetDirectoryName(typeof(WtqPaths).Assembly.Location);

			Console.WriteLine($"GetWtqAppDir() => {_pathToAppDir}");

			if (string.IsNullOrWhiteSpace(_pathToAppDir))
			{
				throw new WtqException("Could not get path to app directory.");
			}
		}

		return _pathToAppDir;
	}

	/// <summary>
	/// Path to where log files are stored.
	/// Windows:    C:/users/username/appdata/local/temp/wtq/logs<br/>
	/// Linux:      /tmp/wtq/logs.
	/// </summary>
	public static string GetWtqLogDir() => GetOrCreateDirectory(Path.Combine(GetWtqTempDir(), "logs"));

	/// <summary>
	/// Path to temporary directory.<br/>
	/// Windows:    C:/users/username/appdata/local/temp/wtq<br/>
	/// Linux:      /tmp/wtq.
	/// </summary>
	public static string GetWtqTempDir() => GetOrCreateDirectory(Path.Combine(Path.GetTempPath(), "wtq"));

	/// <summary>
	/// Make sure the specified <param name="path"/> exists.
	/// </summary>
	private static string GetOrCreateDirectory(string path)
	{
		if (Directory.Exists(path))
		{
			return path;
		}

		try
		{
			Directory.CreateDirectory(path);
		}
		catch (Exception ex)
		{
			throw new WtqException($"Could not create app data directory '{path}': {ex.Message}", ex);
		}

		return path;
	}
}