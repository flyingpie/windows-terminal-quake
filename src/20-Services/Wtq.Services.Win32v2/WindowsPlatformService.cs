using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Wtq.Services.Win32v2;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "This entire project (Win32v2) is Windows-only.")]
public class WindowsPlatformService : PlatformServiceBase
{
	/// <summary>
	/// E.g. "C:\Users\username\AppData\Roaming".
	/// </summary>
	private readonly string _pathToAppDataDir;

	/// <summary>
	/// E.g. "C:\Users\username\AppData\Local\Temp".
	/// </summary>
	private readonly string _pathToTempDir;

	public WindowsPlatformService(
		string? pathToAppDataDir = null,
		string? pathToAppDir = null,
		string? pathToTempDir = null,
		string? pathToUserHomeDir = null)
		: base(
			pathToAppDir: pathToAppDir,
			pathToUserHomeDir: pathToUserHomeDir)
	{
		_pathToAppDataDir = pathToAppDataDir ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		_pathToTempDir = pathToTempDir ?? Path.GetTempPath();
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

	/// <summary>
	/// Try to detect the current color scheme, default to "Light".
	/// </summary>
	public override OsColorMode OsColorMode =>
		IsDarkMode() ?? false ? OsColorMode.Dark : OsColorMode.Light;

	/// <inheritdoc/>
	public override string PathToLogsDir =>
		PathToTempDir;

	/// <summary>
	/// Windows, use the standard temp dir with an app-specific subdir.
	/// For example: "C:/Users/marco/AppData/Local/Temp/wtq".
	/// </summary>
	public override string PathToTempDir =>
		Path.Combine(_pathToTempDir, "wtq").EnsureDirExists();

	/// <summary>
	/// It seems Windows wants its icons as ICO files - dark.
	/// </summary>
	public override string PathToTrayIconDark =>
		Path.Combine(PathToAssetsDir, "nl.flyingpie.wtq-black.ico");

	/// <summary>
	/// It seems Windows wants its icons as ICO files - light.
	/// </summary>
	public override string PathToTrayIconLight =>
		Path.Combine(PathToAssetsDir, "nl.flyingpie.wtq-white.ico");

	/// <inheritdoc/>
	public override string PathToWtqConf
		=> PathsToWtqConfs.FirstOrDefault(Fs.Inst.FileExists) ?? PreferredPathWtqConfig;

	/// <inheritdoc/>
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

				// App data
				_pathToAppDataDir,

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

	/// <inheritdoc/>
	public override string PreferredPathWtqConfig =>
		Path.Combine(_pathToAppDataDir, "wtq", "wtq.jsonc").EnsureFileDirExists();

	private static bool? IsDarkMode()
	{
		// Path to the color settings in the registry
		const string registryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

		try
		{
			using var key = Registry.CurrentUser.OpenSubKey(registryKeyPath);
			if (key != null)
			{
				var appsUseLightTheme = key.GetValue("AppsUseLightTheme");
				var systemUseLightTheme = key.GetValue("SystemUsesLightTheme");

				if (appsUseLightTheme != null)
				{
					return (int)appsUseLightTheme != 0;
				}
				else
				{
					Console.WriteLine("AppsUseLightTheme not found.");
				}

				if (systemUseLightTheme != null)
				{
					return (int)systemUseLightTheme != 0;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error accessing the registry: " + ex.Message);
		}

		return null;
	}
}