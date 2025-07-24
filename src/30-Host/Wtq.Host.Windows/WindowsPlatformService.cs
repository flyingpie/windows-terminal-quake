using System.Collections.Generic;
using System.IO;
using Wtq.Services;
using Wtq.Utils;

namespace Wtq.Host.Windows;

public class WindowsPlatformService : PlatformServiceBase
{
	/// <summary>
	/// When looking for the existence of a file and whether it's executable, we consider these extensions.
	/// </summary>
	public override string[] ExecutableExtensions => [string.Empty, ".exe", ".bat", ".cmd"];

	/// <summary>
	/// On Windows, default to a named pipe.
	/// </summary>
	public override ICollection<string> DefaultApiUrls =>
		["http://pipe:/wtq"];

	public override string PlatformName { get; }

	public override string PathToAppDir { get; }

	public override string PathToAppIcon { get; }

	public override string PathToLogsDir { get; }

	/// <summary>
	/// Windows, use the standard temp dir with an app-specific subdir.
	/// For example: "C:/Users/marco/AppData/Local/Temp/wtq".
	/// </summary>
	public override string PathToTempDir =>
		Path.Combine(Path.GetTempPath(), "wtq").GetOrCreateDirectory();

	/// <summary>
	/// It seems Windows wants its icons as ICO files.
	/// </summary>
	public override string PathToTrayIcon =>
		Path.Combine(PathToAssetsDir, "icon-v2-256-nopadding.ico");

	public override string PathToWtqConf { get; }

	public override ICollection<string> PathsToWtqConfs { get; }

	public override string PreferredPathWtqConfig { get; }
}