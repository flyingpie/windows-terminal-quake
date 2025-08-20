using System.Runtime.InteropServices;

namespace Wtq.Services;

/// <summary>
/// Implements a bunch of stuff from <see cref="IPlatformService"/>, that's shared across multiple platforms.
/// </summary>
public abstract class PlatformServiceBase : IPlatformService
{
	private string? _pathToWtqConf;

	/// <summary>
	/// Paramaters are used for mocking in tests, otherwise they're automatically set in the constructor.
	/// </summary>
	protected PlatformServiceBase(
		string? pathToAppDir = null,
		string? pathToUserHomeDir = null)
	{
		// Path to directory where wtq is installed (where wtq or wtq.exe lives).
		PathToAppDir = pathToAppDir ?? Path.GetDirectoryName(GetType().Assembly.Location)?.EmptyOrWhiteSpaceToNull() ??
			throw new WtqException("Could not get path to app directory.");

		// Windows: C:/users/username
		// Linux: /home/username
		PathToUserHomeDir = pathToUserHomeDir ??
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
	}

	/// <inheritdoc/>
	public abstract string PlatformName { get; }

	/// <inheritdoc/>
	public abstract ICollection<string> DefaultApiUrls { get; }

	/// <summary>
	/// When looking for the existence of a file and whether it's executable, we consider these extensions.
	/// </summary>
	public abstract string[] ExecutableExtensions { get; }

	/// <inheritdoc/>
	public abstract OsColorMode OsColorMode { get; }

	/// <inheritdoc/>
	public virtual string PathToAppDir { get; }

	/// <inheritdoc/>
	public virtual string PathToAssetsDir =>
		Path.Combine(PathToAppDir, "assets");

	/// <inheritdoc/>
	public abstract string PathToLogsDir { get; }

	/// <inheritdoc/>
	public abstract string PathToTempDir { get; }

	/// <inheritdoc/>
	public abstract string PathToTrayIconDark { get; }

	/// <inheritdoc/>
	public abstract string PathToTrayIconLight { get; }

	/// <inheritdoc/>
	public virtual string PathToUserHomeDir { get; }

	/// <inheritdoc/>
	public virtual string PathToWtqConf
	{
		get
		{
			// See if we already determined the path
			if (_pathToWtqConf != null)
			{
				return _pathToWtqConf;
			}

			// Look for existing path
			foreach (var p in PathsToWtqConfs)
			{
				Log.LogInformation("Looking for WTQ settings file at path '{Path}'", p);

				if (Fs.Inst.FileExists(p))
				{
					Log.LogInformation("Found WTQ settings file at path '{Path}'", p);

					_pathToWtqConf = p;
					return _pathToWtqConf;
				}
			}

			// If no existing file was found, generate an example file at the preferred location, and use that.
			// Note that the example config is set up to pop the UI, so the user gets a decent out-of-box experience.
			_pathToWtqConf = PreferredPathWtqConfig;
			Log.LogInformation("No settings file found, generating an example file at '{Path}'", _pathToWtqConf);
			Fs.Inst.WriteAllText(_pathToWtqConf, GetExampleWtqJsonc());
			return _pathToWtqConf;
		}
	}

	/// <inheritdoc/>
	public virtual string PathToWtqConfDir =>
		Path.GetDirectoryName(PathToWtqConf) ?? throw new WtqException($"Could not determine directory of path '{PathToWtqConf}'.");

	private string GetExampleWtqJsonc()
	{
		var pathToExampleWtqJsonc = Path.Combine(PathToAssetsDir, "wtq.example.jsonc");
		if (Fs.Inst.FileExists(pathToExampleWtqJsonc))
		{
			return Fs.Inst.ReadAllText(pathToExampleWtqJsonc);
		}

		Log.LogWarning("Example settings file was not found at expected path '{Path}', returning minimal example file", pathToExampleWtqJsonc);

		return
			"""
			{
				// Something went wrong generating an example file, please see the docs and the autocompletion for this file instead.
				// Also, I'd appreciate it a ton if you could report this issue.
				"$schema": "wtq.schema.json",

				"Apps": [
					// Apps go here
					// {
					//   "Name": "App1"
					// }
				]
			}
			""";
	}

	/// <inheritdoc/>
	public abstract ICollection<string> PathsToWtqConfs { get; }

	/// <inheritdoc/>
	public abstract string PreferredPathWtqConfig { get; }

	/// <summary>
	/// Possible names for the settings file. Used for building potential paths.
	/// </summary>
	protected virtual ICollection<string> WtqConfNames =>
	[
		"wtq",
		".wtq",
	];

	/// <summary>
	/// Possible extensions for the settings file. Used for building potential paths.
	/// </summary>
	public virtual ICollection<string> WtqConfExtensions =>
	[
		"json",
		"jsonc",
		"json5",
	];

	/// <inheritdoc/>
	public virtual Process CreateProcess(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		// Validate filename
		if (string.IsNullOrWhiteSpace(opts.FileName))
		{
			throw new InvalidOperationException($"Cannot start process for app '{opts.Name}': missing required property '{nameof(opts.FileName)}'");
		}

		// Build start info
		var startInfo = new ProcessStartInfo()
		{
			FileName = opts.FileName, Arguments = opts.Arguments, WorkingDirectory = opts.WorkingDirectory,
		};

		// Arguments
		foreach (var arg in opts.ArgumentList)
		{
			// Skip empty arguments
			if (string.IsNullOrWhiteSpace(arg.Argument))
			{
				continue;
			}

			// Expand environment variables, also includes "~" for home dir
			var exp = arg.Argument.ExpandEnvVars();

			// Log, very useful when using environment variables in arguments
			Log.LogDebug("Adding process argument '{ArgumentOriginal}', expanded to '{ArgumentExpanded}'", arg, exp);

			startInfo.ArgumentList.Add(exp);
		}

		return new Process()
		{
			StartInfo = startInfo,
		};
	}

	/// <inheritdoc/>
	public string? ResolvePath(string fileName)
	{
		// If the file exists without any further steps (maybe it's already absolute,
		// or the file exists in the working), just resolve and return.
		if (File.Exists(fileName))
		{
			return Path.GetFullPath(fileName);
		}

		// Fetch the "PATH" environment variable for locations that can contain callable executables
		var values = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;

		// Loop through all the paths in "PATH"
		foreach (var path in values.Split(Path.PathSeparator))
		{
			// Combine specified filename with current path segment
			var fullPath = Path.Combine(path, fileName);

			// See if the file exists here
			if (File.Exists(fullPath))
			{
				return fullPath;
			}
		}

		return null;
	}

	/// <inheritdoc/>
	public virtual bool IsCallable(string? workingDirectory, string fileName)
	{
		// See if the filename is callable with any of the OS-specific extensions (will be
		// empty for Linux, and ".exe", ".bat" and such for Windows)
		return ExecutableExtensions.Any(ex => ResolvePath(fileName + ex) != null);
	}

	/// <inheritdoc/>
	public virtual bool ShouldUsePollingFileWatcherForPath(string path)
	{
		Guard.Against.NullOrWhiteSpace(path);

		if (!File.Exists(path))
		{
			throw new InvalidOperationException($"No such file at path '{path}'.");
		}

		return new FileInfo(path).Attributes.HasFlag(FileAttributes.ReparsePoint);
	}

	/// <inheritdoc/>
	public virtual void OpenFileOrDirectory(string path)
	{
		Guard.Against.NullOrWhiteSpace(path);

		try
		{
			Process.Start(
				new ProcessStartInfo()
				{
					FileName = path, UseShellExecute = true,
				});
		}
		catch (Exception ex)
		{
			Log.LogWarning(ex, "Could not open file or directory {Path}: {Message}", path, ex.Message);
		}
	}

	/// <inheritdoc/>
	public virtual void OpenUrl(Uri url)
	{
		Guard.Against.Null(url);

		try
		{
			Process.Start(url.ToString());
		}
		catch (Exception ex)
		{
			Log.LogWarning(ex, "Could not open url {Url}: {Message}", url, ex.Message);

			// Hack because of this: https://github.com/dotnet/corefx/issues/10361
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				var urlstr = url.ToString().Replace("&", "^&", StringComparison.Ordinal);
				Process.Start(
					new ProcessStartInfo(urlstr)
					{
						UseShellExecute = true,
					});
			}
		}
	}

	/// <summary>
	/// We're creating a logger on the fly, because the platform service is initialized soon enough,
	/// that the logger may not be ready at first.
	/// </summary>
	protected ILogger Log => Wtq.Utils.Log.For(GetType());
}