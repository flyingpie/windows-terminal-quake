using Microsoft.Win32;
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

	public override OsColorMode OsColorMode =>
		IsDarkMode() ?? false ? OsColorMode.Dark : OsColorMode.Light;

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
	/// It seems Windows wants its icons as ICO files - dark.
	/// </summary>
	public override string PathToTrayIconDark =>
		Path.Combine(PathToAssetsDir, "icon-v2-256-nopadding.ico");

	/// <summary>
	/// It seems Windows wants its icons as ICO files - light.
	/// </summary>
	public override string PathToTrayIconLight =>
		Path.Combine(PathToAssetsDir, "icon-v2-256-nopadding.ico"); // TODO

	/// <inheritdoc/>
	public override string PathToWtqConf { get; }

	/// <inheritdoc/>
	public override ICollection<string> PathsToWtqConfs { get; }

	/// <inheritdoc/>
	public override string PreferredPathWtqConfig { get; }

	public static bool? IsDarkMode()
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
					var isAppLightMode = (int)appsUseLightTheme != 0;
					Console.WriteLine("App color mode: " + (isAppLightMode ? "Light" : "Dark"));
					return isAppLightMode;
				}
				else
				{
					Console.WriteLine("AppsUseLightTheme not found.");
				}

				if (systemUseLightTheme != null)
				{
					var isSystemLightMode = (int)systemUseLightTheme != 0;
					Console.WriteLine("System color mode: " + (isSystemLightMode ? "Light" : "Dark"));
					return isSystemLightMode;
				}
				else
				{
					Console.WriteLine("SystemUsesLightTheme not found.");
				}
			}
			else
			{
				Console.WriteLine("Registry key not found.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error accessing the registry: " + ex.Message);
		}

		return null;
	}
}