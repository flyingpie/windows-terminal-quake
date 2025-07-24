namespace Wtq.Services;

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

	public abstract string PlatformName { get; }

	public abstract ICollection<string> DefaultApiUrls { get; }

	public abstract string[] ExecutableExtensions { get; }

	public virtual string PathToAppDir { get; }

	public virtual string PathToAssetsDir =>
		Path.Combine(PathToAppDir, "assets");

	public abstract string PathToLogsDir { get; }

	public abstract string PathToAppIcon { get; }

	public abstract string PathToTempDir { get; }

	public abstract string PathToTrayIcon { get; }

	public virtual string PathToUserHomeDir { get; }

	public virtual string PathToWtqConf { get; }

	/// <inheritdoc/>
	public virtual string PathToWtqConfDir =>
		Path.GetDirectoryName(PathToWtqConf) ?? throw new WtqException($"Could not determine directory of path '{PathToWtqConf}'.");

	public abstract ICollection<string> PathsToWtqConfs { get; }

	public abstract string PreferredPathWtqConfig { get; }

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

	// public bool ExistsOnPath(string fileName)
	// {
	// 	return GetFullPath(fileName) != null;
	// }

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