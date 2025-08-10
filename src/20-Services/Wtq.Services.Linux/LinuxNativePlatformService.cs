using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Wtq.Services.Linux;

public class LinuxNativePlatformService : PlatformServiceBase
{
	public LinuxNativePlatformService(
		string? pathToAppDir = null,
		string? pathToUserHomeDir = null)
		: base(
			pathToAppDir: pathToAppDir,
			pathToUserHomeDir: pathToUserHomeDir)
	{
	}

	/// <inheritdoc/>
	public override string PlatformName => "Linux";

	/// <summary>
	/// On Linux, default to a Unix socket.
	/// </summary>
	public override ICollection<string> DefaultApiUrls =>
		[$"http://unix:{$"{XDG_RUNTIME_DIR}/wtq/wtq.sock".EnsureFileDirExists()}"];

	/// <summary>
	/// On Linux, executable files generally don't have extensions.<br/>
	/// Some exceptions exist, like ".sh", but these require the extension to be part of the filename when executing.
	/// </summary>
	public override string[] ExecutableExtensions =>
		[string.Empty];

	/// <summary>
	/// Not supported yet on Linux, always returning <see cref="OsColorMode.Unknown"/>.
	/// </summary>
	public override OsColorMode OsColorMode => OsColorMode.Unknown;

	public override string PathToLogsDir =>
		Path.Combine(PathToTempDir);

	/// <summary>
	/// Native Linux, use XDG_STATE_HOME with an app-specific subdirectory.
	/// For example: "/home/user/.local/state/wtq".
	/// </summary>
	public override string PathToTempDir =>
		Path.Combine(XDG_STATE_HOME, "wtq");

	/// <summary>
	/// On native Linux, use a physical path to the tray icon, as an SVG.
	/// </summary>
	public override string PathToTrayIconDark =>
		Path.Combine(PathToAppDir, "assets", "nl.flyingpie.wtq-black.svg").AssertFileExists();

	/// <summary>
	/// On native Linux, use a physical path to the tray icon, as an SVG.
	/// </summary>
	public override string PathToTrayIconLight =>
		Path.Combine(PathToAppDir, "assets", "nl.flyingpie.wtq-white.svg").AssertFileExists();

	[SuppressMessage("Critical Code Smell", "S2365:Properties should not make collection or array copies", Justification = "MvdO: It's fine, doesn't get called often.")]
	public override ICollection<string> PathsToWtqConfs
	{
		get
		{
			var res = new List<string?>();

			// Environment variable
			res.Add(WtqEnv.WtqConfigFile);

			foreach (var path in new[]
			{
				// Next to the app executable
				PathToAppDir,

				// XDG home, defaults to user home (subfolder)
				Path.Combine(XDG_CONFIG_HOME, "wtq"),

				// XDG home, defaults to user home (bare)
				XDG_CONFIG_HOME,

				// Explicitly user home
				PathToUserHomeDir,
			})
			{
				foreach (var name in WtqConfNames)
				{
					foreach (var ext in WtqConfExtensions)
					{
						res.Add(Path.Combine(path, $"{name}.{ext}"));
					}
				}
			}

			return res
				.Where(e => !string.IsNullOrWhiteSpace(e))
				.Select(e => e!)
				.Distinct()
				.ToList();
		}
	}

	public override string PreferredPathWtqConfig =>
		EnvUtils.GetEnvVarOrDefault(WtqEnv.Names.Config, Path.Combine(XDG_CONFIG_HOME, "wtq.jsonc"));

	/// <summary>
	/// Linux version uses xdg-open for opening files and directories.
	/// </summary>
	public override void OpenFileOrDirectory(string path)
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
			Log.LogWarning(ex, "Could not open file, directory or url {Path}: {Message}", path, ex.Message);
		}
	}

	/// <summary>
	/// Linux version uses xdg-open for opening urls.
	/// </summary>
	public override void OpenUrl(Uri url)
	{
		OpenFileOrDirectory(url.ToString());
	}

	protected string XDG_CONFIG_HOME =>
		EnvUtils.GetEnvVarOrDefault("XDG_CONFIG_HOME", Path.Combine(PathToUserHomeDir, ".config"));

	protected string XDG_RUNTIME_DIR =>
		EnvUtils.GetEnvVarOrDefault("XDG_RUNTIME_DIR", XDG_STATE_HOME);

	/// <summary>
	/// $XDG_STATE_HOME defines the base directory relative to which user-specific state files should be stored.
	/// If $XDG_STATE_HOME is either not set or empty, a default equal to $HOME/.local/state should be used.
	///
	/// The $XDG_STATE_HOME contains state data that should persist between (application) restarts, but that is
	/// not important or portable enough to the user that it should be stored in $XDG_DATA_HOME.
	///
	/// It may contain:
	/// - actions history (logs, history, recently used files, …)
	/// - current state of the application that can be reused on a restart (view, layout, open files, undo history, …).
	/// </summary>
	protected string XDG_STATE_HOME =>
		EnvUtils.GetEnvVarOrDefault("XDG_STATE_HOME", Path.Combine(PathToUserHomeDir, ".local", "state"));
}