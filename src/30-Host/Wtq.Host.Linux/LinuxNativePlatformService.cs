using System.IO;
using Wtq.Services;
using Wtq.Utils;

namespace Wtq.Host.Linux;

public class LinuxNativePlatformService : PlatformServiceBase
{
	public const string Config = "WTQ_CONFIG_FILE";

	public override string PlatformName { get; } = "Linux Native";

	/// <summary>
	/// When looking for the existence of a file and whether it's executable, we consider these extensions.
	/// </summary>
	public override string[] ExecutableExtensions => [string.Empty];

	public override string PathToLogsDir { get; }

	public override string PathToAppIcon { get; }

	/// <summary>
	/// Native Linux, use XDG_STATE_HOME with an app-specific subdir.
	/// For example: "/home/user/.local/state/wtq".
	/// </summary>
	public override string PathToTempDir =>
		Path.Combine(XDG.XDG_STATE_HOME, "wtq").GetOrCreateDirectory();

	/// <summary>
	///
	// _log.LogDebug("Running bare Linux, using icon path of tray icon");
	/// </summary>
	public override string PathToTrayIcon =>
		Path.Combine(PathToAppDir, "assets", "nl.flyingpie.wtq-white.svg").AssertFileExists();

	public override string PathToWtqConf { get; }

	public override ICollection<string> PathsToWtqConfs { get; }

	public override string PreferredPathWtqConfig { get; }
}