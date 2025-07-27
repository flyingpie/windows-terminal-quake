namespace Wtq.Services;

/// <summary>
/// Implements a bunch of stuff of <see cref="IPlatformService"/>, that's shared across multiple platforms.
/// </summary>
public abstract class PlatformServiceBase : IPlatformService
{
	private readonly ILogger _log;

	protected PlatformServiceBase(
		string? pathToAppDir = null,
		string? pathToUserHomeDir = null)
	{
		_log = Log.For(GetType());

		PathToAppDir = pathToAppDir ?? Path.GetDirectoryName(GetType().Assembly.Location)?.EmptyOrWhiteSpaceToNull() ??
			throw new WtqException("Could not get path to app directory.");

		PathToUserHomeDir = pathToUserHomeDir ??
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
	}

	/// <inheritdoc/>
	public abstract string PlatformName { get; }

	/// <inheritdoc/>
	public abstract ICollection<string> DefaultApiUrls { get; }

	/// <inheritdoc/>
	public abstract OsColorMode OsColorMode { get; }

	/// <summary>
	/// When looking for the existence of a file and whether it's executable, we consider these extensions.
	/// </summary>
	public abstract string[] ExecutableExtensions { get; }

	/// <inheritdoc/>
	public virtual string PathToAppDir { get; }

	// /// <inheritdoc/>
	// public abstract string PathToAppIcon { get; }

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
	public virtual string PathToWtqConf { get; }

	/// <inheritdoc/>
	public virtual string PathToWtqConfDir =>
		Path.GetDirectoryName(PathToWtqConf) ?? throw new WtqException($"Could not determine directory of path '{PathToWtqConf}'.");

	/// <inheritdoc/>
	public abstract ICollection<string> PathsToWtqConfs { get; }

	/// <inheritdoc/>
	public abstract string PreferredPathWtqConfig { get; }

	public virtual ICollection<string> WtqConfNames =>
	[
		"wtq",
		".wtq",
	];

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

		if (string.IsNullOrWhiteSpace(opts.FileName))
		{
			throw new InvalidOperationException($"Cannot start process for app '{opts.Name}': missing required property '{nameof(opts.FileName)}'");
		}

		var startInfo = new ProcessStartInfo()
		{
			FileName = opts.FileName, Arguments = opts.Arguments, WorkingDirectory = opts.WorkingDirectory,
		};

		// Arguments
		foreach (var arg in opts.ArgumentList)
		{
			if (string.IsNullOrWhiteSpace(arg.Argument))
			{
				continue;
			}

			var exp = arg.Argument.ExpandEnvVars();

			_log.LogDebug("Adding process argument '{ArgumentOriginal}', expanded to '{ArgumentExpanded}'", arg, exp);

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
		if (File.Exists(fileName))
		{
			return Path.GetFullPath(fileName);
		}

		var values = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
		foreach (var path in values.Split(Path.PathSeparator))
		{
			var fullPath = Path.Combine(path, fileName);
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
		foreach (var ext in ExecutableExtensions)
		{
			var path = fileName + ext;

			if (File.Exists(path))
			{
				return true;
			}

			if (ResolvePath(path) != null)
			{
				return true;
			}
		}

		return false;
	}

	/// <inheritdoc/>
	public virtual bool ShouldUsePollingFileWatcherForPath(string path)
	{
		Guard.Against.NullOrWhiteSpace(path);

		if (!File.Exists(path))
		{
			throw new InvalidOperationException($"No such file at path '{path}'.");
		}

		var pathInfo = new FileInfo(path);

		return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
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
			_log.LogWarning(ex, "Could not open file or directory {Path}: {Message}", path, ex.Message);
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
			_log.LogWarning(ex, "Could not open url {Url}: {Message}", url, ex.Message);

			// // Hack because of this: https://github.com/dotnet/corefx/issues/10361
			// if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			// {
			// 	var urlstr = url.ToString().Replace("&", "^&", StringComparison.Ordinal);
			// 	Process.Start(
			// 		new ProcessStartInfo(urlstr)
			// 		{
			// 			UseShellExecute = true,
			// 		});
			// }
			// else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			// {
			// 	Process.Start("xdg-open", url.ToString());
			// }
			// else
			// {
			// 	throw;
			// }
		}
	}
}