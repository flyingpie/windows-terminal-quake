namespace Wtq.Services.CLI.Commands;

[Command]
public class InfoCommand : ICommand
{
	public void Execute()
	{
		Console.WriteLine($"Platform:......................................{PlatformName}");

		Console.WriteLine($"-- Paths --------------------------------------------------------");
		Console.WriteLine($"{nameof(WtqPaths.AppData)}:....................{WtqPaths.AppData}");
		Console.WriteLine($"{nameof(WtqPaths.AppDataWtq)}:.................{WtqPaths.AppDataWtq}");
		Console.WriteLine($"{nameof(WtqPaths.PreferredWtqConfigPath)}:.....{WtqPaths.PreferredWtqConfigPath}");
		Console.WriteLine($"{nameof(WtqPaths.UserHome)}:...................{WtqPaths.UserHome}");
		Console.WriteLine($"{nameof(WtqPaths.GetWtqAppDir)}:...............{WtqPaths.GetWtqAppDir()}");
		Console.WriteLine($"{nameof(WtqPaths.WtqConfigFromEnvVar)}:........{WtqPaths.WtqConfigFromEnvVar}");
		Console.WriteLine($"{nameof(WtqPaths.WtqConfigPaths)}:.............{string.Join("\n-", WtqPaths.WtqConfigPaths)}");
		Console.WriteLine($"{nameof(WtqPaths.GetWtqLogDir)}:...............{WtqPaths.GetWtqLogDir()}");
		Console.WriteLine($"{nameof(WtqPaths.GetWtqTempDir)}:..............{WtqPaths.GetWtqTempDir()}");
		Console.WriteLine($"-- /Paths -------------------------------------------------------");
	}

	private static string PlatformName
	{
		get
		{
			if (Os.IsFlatpak)
			{
				return "Flatpak";
			}

			if (Os.IsLinux)
			{
				return "Linux";
			}

			if (Os.IsWindows)
			{
				return "Windows";
			}

			return "Unknown";
		}
	}
}