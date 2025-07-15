using System.Diagnostics;
using System.IO;
using Wtq.Configuration;
using Wtq.Exceptions;
using Wtq.Services;
using Wtq.Utils;

namespace Wtq.Host.Linux;

public class LinuxFlatpakPlatformService : IPlatformService
{
	/// <inheritdoc/>
	public string PlatformName => "Flatpak";

	/// <inheritdoc/>
	public string PathToAppDir =>
		Path.GetDirectoryName(typeof(WtqPaths).Assembly.Location)?.EmptyOrWhiteSpaceToNull() ??
		throw new WtqException("Could not get path to app directory.");

	/// <inheritdoc/>
	public string PathToLogs => WtqPaths.WtqLogDir;

	/// <inheritdoc/>
	public string PathToAppIcon { get; }

	/// <inheritdoc/>
	public string PathToTrayIcon => "nl.flyingpie.wtq-white";

	/// <inheritdoc/>
	public string PathToUserHome => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

	/// <inheritdoc/>
	public string PathToWtqConf => WtqPaths.PreferredWtqConfigPath;

	/// <inheritdoc/>
	public string PathToWtqConfDir => Path.GetDirectoryName(PathToWtqConf) ?? throw new WtqException($"Could not determine directory of path '{PathToWtqConf}'.");

	/// <inheritdoc/>
	public ICollection<string> PathsToWtqConfs =>
	[
		// In XDG config dir.
		Path.Combine(PathToUserHome, ".config", "wtq.json"),
		Path.Combine(PathToUserHome, ".config", "wtq.jsonc"),
		Path.Combine(PathToUserHome, ".config", "wtq.json5"),
	];

	/// <inheritdoc/>
	public string PreferredPathWtqConfig => throw new NotImplementedException();

	/// <inheritdoc/>
	public Process CreateProcess(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		if (string.IsNullOrWhiteSpace(opts.FileName))
		{
			throw new InvalidOperationException($"Cannot start process for app '{opts.Name}': missing required property '{nameof(opts.FileName)}'.");
		}

		var startInfo = new ProcessStartInfo()
		{
			FileName = "flatpak-spawn",
		};

		startInfo.ArgumentList.Add("--host");

		// Working directory
		if (!string.IsNullOrWhiteSpace(opts.WorkingDirectory))
		{
			startInfo.ArgumentList.Add("--directory");
			startInfo.ArgumentList.Add(opts.WorkingDirectory);
		}

		// Filename
		startInfo.ArgumentList.Add(opts.FileName);

		// Arguments
		foreach (var arg in opts.ArgumentList ?? [])
		{
			if (string.IsNullOrWhiteSpace(arg.Argument))
			{
				continue;
			}

			var exp = arg.Argument.ExpandEnvVars();

			// _log.LogDebug("Adding process argument '{ArgumentOriginal}', expanded to '{ArgumentExpanded}'", arg.Argument, exp);

			startInfo.ArgumentList.Add(exp);
		}

		return new Process()
		{
			StartInfo = startInfo,
		};
	}

	/// <inheritdoc/>
	public bool IsCallable(string? workingDirectory, string fileName)
	{
		return false; // TODO
	}

	/// <inheritdoc/>
	public bool ShouldUsePollingFileWatcherForPath(string path)
	{
		return false; // TODO
	}

	/// <inheritdoc/>
	public void OpenFileOrDirectory(string path)
	{
		// TODO
	}

	/// <inheritdoc/>
	public void OpenUrl(Uri url)
	{
		// TODO
	}
}