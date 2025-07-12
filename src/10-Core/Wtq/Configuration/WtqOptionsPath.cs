using System.Runtime.InteropServices;

namespace Wtq.Configuration;

public class WtqOptionsPath
{
	public static readonly WtqOptionsPath Instance = new();

	private readonly ILogger _log = Log.For<WtqOptionsPath>();

	private WtqOptionsPath()
	{
		var path = FindPathToExistingWtqConf();

		if (path == null)
		{
			path = WtqPaths.PreferredWtqConfigPath;
			_log.LogInformation("No WTQ configuration found, generating example config at path '{Path}'", path);

			// Make sure the directory exists.
			Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path)!);

			// Write example config.
			File.WriteAllText(path, GetExampleWtqConf());
		}

		Path = path;
	}

	public string Path { get; private set; }

	/// <summary>
	/// Returns example contents for "wtq.jsonc", that we can use to start off the user.<br/>
	/// For use when WTQ starts with no settings file present.
	/// </summary>
	private string GetExampleWtqConf()
	{
		// On Linux, default to Konsole (as we currently only support KDE 5 & 6).
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			_log.LogInformation("Running on Linux, using Konsole example config");
			return Resources.Resources.wtq_example_konsole;
		}

		// On Windows, check if Windows Terminal is available. If so, use an example config with WT configured.
		if (Os.ExistsOnPath("wt.exe"))
		{
			_log.LogInformation("Found Windows Terminal to be installed, using WT example config");
			return Resources.Resources.wtq_example_wt;
		}

		// On Windows, with (seemingly) no Windows Terminal installed, fallback to PowerShell.
		_log.LogInformation("Did not find Windows Terminal to be installed, using PowerShell example config");
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