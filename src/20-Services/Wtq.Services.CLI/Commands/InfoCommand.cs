namespace Wtq.Services.CLI.Commands;

[Command]
public class InfoCommand(IPlatformService platform) : ICommand
{
	public void Execute()
	{
		Console.WriteLine($"-- App ----------------------------------------------------------");
		Console.WriteLine($"Platform:......................................{platform.PlatformName}");
		Console.WriteLine($"App version:...................................{WtqConstants.AppVersion}");
		Console.WriteLine($"File version:..................................{WtqConstants.AppFileVersion}");
		Console.WriteLine($"Informational version:.........................{WtqConstants.AppInformationalVersion}");
		Console.WriteLine($"Build configuration:...........................{WtqConstants.BuildConfiguration}");
		Console.WriteLine($"Build date:....................................{WtqConstants.BuildDate.ToString("s")}");
		Console.WriteLine($"Git branch:....................................{WtqConstants.GitBranch}");
		Console.WriteLine($"Git commit:....................................{WtqConstants.GitCommit}");
		Console.WriteLine($"-- Paths --------------------------------------------------------");
		// Console.WriteLine($"{nameof(WtqPaths.AppData)}:....................{WtqPaths.AppData}"); // TODO: Fix
		// Console.WriteLine($"{nameof(WtqPaths.AppDataWtq)}:.................{WtqPaths.AppDataWtq}");
		// Console.WriteLine($"{nameof(WtqPaths.PreferredWtqConfigPath)}:.....{WtqPaths.PreferredWtqConfigPath}");
		// Console.WriteLine($"{nameof(WtqPaths.UserHome)}:...................{WtqPaths.UserHome}");
		// Console.WriteLine($"{nameof(WtqPaths.GetWtqAppDir)}:...............{WtqPaths.GetWtqAppDir()}");
		// Console.WriteLine($"{nameof(WtqPaths.WtqConfigFromEnvVar)}:........{WtqPaths.WtqConfigFromEnvVar}");
		// Console.WriteLine($"{nameof(WtqPaths.WtqConfigPaths)}:.............{string.Join("\n  -", WtqPaths.WtqConfigPaths)}");
		// Console.WriteLine($"{nameof(WtqPaths.GetWtqLogDir)}:...............{WtqPaths.GetWtqLogDir()}");
		// Console.WriteLine($"{nameof(WtqPaths.GetWtqTempDir)}:..............{WtqPaths.GetWtqTempDir()}");
		Console.WriteLine($"-----------------------------------------------------------------");
	}
}