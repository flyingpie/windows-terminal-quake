using Wtq.Configuration;

namespace Wtq.Services.Linux;

public class LinuxFlatpakPlatformService : LinuxNativePlatformService
{
	/// <inheritdoc/>
	public override string PlatformName => "Flatpak";

	// /// <inheritdoc/>
	// public override string PathToAppIcon { get; }

	/// <summary>
	/// Flatpak, use XDG_STATE_HOME without an app-specific subdir (since the entire directory is already app-specific).
	/// For example: "/home/user/.var/app/nl.flyingpie.wtq/.local/state".
	/// </summary>
	public override string PathToTempDir =>
		XDG_STATE_HOME;

	/// <inheritdoc/>
	///		_log.LogDebug("Running in Flatpak, using icon name of tray icon (i.e., not the full path)");
	public override string PathToTrayIcon =>
		"nl.flyingpie.wtq-white";

	// TODO: I'm expecting preper implementation of XDG spec to make this unnecessary.
	// /// <inheritdoc/>
	// public override ICollection<string> PathsToWtqConfs =>
	// [
	// 	// In XDG config dir.
	// 	// TODO
	// 	Path.Combine(PathToUserHome, ".config", "wtq.json"),
	// 	Path.Combine(PathToUserHome, ".config", "wtq.jsonc"),
	// 	Path.Combine(PathToUserHome, ".config", "wtq.json5"),
	//
	// 	..base.PathsToWtqConfs
	// ];

	/// <inheritdoc/>
	public override Process CreateProcess(WtqAppOptions opts)
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
	public override bool IsCallable(string? workingDirectory, string fileName)
	{
		return false; // TODO
	}

	/// <inheritdoc/>
	public override void OpenFileOrDirectory(string path)
	{
		// TODO
	}

	/// <inheritdoc/>
	public override void OpenUrl(Uri url)
	{
		// TODO
	}
}