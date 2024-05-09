namespace Wtq.Configuration;

public class WtqOptionsPath
{
	public static readonly WtqOptionsPath Instance = new();

	private readonly ILogger _log = Log.For(typeof(WtqOptionsPath));

	private WtqOptionsPath()
	{
		var path = FindPathToExistingWtqConf();

		if (path == null)
		{
			path = WtqPaths.PreferredWtqConfigPath;
			_log.LogInformation("No WTQ configuration found, generating example config at path '{Path}'", path);

			File.WriteAllText(path, GetExampleWtqConf());
		}

		Path = path;
	}

	public string Path { get; private set; }

	// TODO: Linux equivalents.
	private string GetExampleWtqConf()
	{
		if (Os.ExistsOnPath("wt.exe"))
		{
			_log.LogInformation("Found Windows Terminal to be installed, using WT example config.");
			return Resources.Resources.wtq_example_wt;
		}

		_log.LogInformation("Did not find Windows Terminal to be installed, using PowerShell example config.");
		return Resources.Resources.wtq_example_ps;
	}

	private string? FindPathToExistingWtqConf()
	{
		foreach (var path in WtqPaths.WtqConfigPaths)
		{
			if (File.Exists(path))
			{
				_log.LogInformation("Found config file at path '{Path}'", path);
				return path;
			}
			else
			{
				_log.LogInformation("No config file found at path '{Path}'", path);
			}
		}

		_log.LogWarning("No config file found at any of the possible paths");
		return null;
	}
}