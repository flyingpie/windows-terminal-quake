using System.IO;

namespace Wtq.Services.Win32v2;

public class WindowsPlatformService : PlatformServiceBase
{
	public WindowsPlatformService(
		string? pathToAppDir = null,
		string? pathToUserHomeDir = null)
		: base(
			pathToAppDir: pathToAppDir,
			pathToUserHomeDir: pathToUserHomeDir)
	{
	}

	/// <inheritdoc/>
	public override string PlatformName => "Windows";

	/// <inheritdoc/>
	public override string[] ExecutableExtensions =>
		[string.Empty, ".exe", ".bat", ".cmd"];

	/// <summary>
	/// On Windows, default to a named pipe.
	/// </summary>
	public override ICollection<string> DefaultApiUrls =>
		["http://pipe:/wtq"];

	// /// <inheritdoc/>
	// public override string PathToAppIcon { get; }

	/// <inheritdoc/>
	public override string PathToLogsDir =>
		Path.Combine(PathToTempDir, "logs").EnsureDirExists();

	/// <summary>
	/// Windows, use the standard temp dir with an app-specific subdir.
	/// For example: "C:/Users/marco/AppData/Local/Temp/wtq".
	/// </summary>
	public override string PathToTempDir =>
		Path.Combine(Path.GetTempPath(), "wtq").EnsureDirExists();

	/// <summary>
	/// It seems Windows wants its icons as ICO files.
	/// </summary>
	public override string PathToTrayIcon =>
		Path.Combine(PathToAssetsDir, "icon-v2-256-nopadding.ico");

	/// <inheritdoc/>
	public override string PathToWtqConf { get; }

	/// <inheritdoc/>
	public override ICollection<string> PathsToWtqConfs { get; }

	/// <inheritdoc/>
	public override string PreferredPathWtqConfig { get; }
}