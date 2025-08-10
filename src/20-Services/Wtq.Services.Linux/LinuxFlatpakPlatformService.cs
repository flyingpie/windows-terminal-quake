using Wtq.Configuration;

namespace Wtq.Services.Linux;

public class LinuxFlatpakPlatformService : LinuxNativePlatformService
{
	/// <inheritdoc/>
	public override string PlatformName => "Flatpak";

	/// <summary>
	/// Flatpak, use XDG_STATE_HOME without an app-specific subdir (since the entire directory is already app-specific).
	/// For example: "/home/user/.var/app/nl.flyingpie.wtq/.local/state".
	/// </summary>
	public override string PathToTempDir =>
		XDG_STATE_HOME;

	/// <summary>
	/// Running in Flatpak, using icon name of tray icon (i.e., not the full path) - Dark.
	/// </summary>
	public override string PathToTrayIconDark =>
		"nl.flyingpie.wtq-black";

	/// <summary>
	/// Running in Flatpak, using icon name of tray icon (i.e., not the full path) - Light.
	/// </summary>
	public override string PathToTrayIconLight =>
		"nl.flyingpie.wtq-white";

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

			Log.LogDebug("Adding process argument '{ArgumentOriginal}', expanded to '{ArgumentExpanded}'", arg.Argument, exp);

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