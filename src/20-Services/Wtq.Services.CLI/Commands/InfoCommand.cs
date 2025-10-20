namespace Wtq.Services.CLI.Commands;

[Command(Parent = typeof(AppRootCommand))]
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
		Console.WriteLine($"OS color mode:.................................{platform.OsColorMode}");

		Console.WriteLine($"-- Paths --------------------------------------------------------");
		Console.WriteLine($"PreferredPathToWtqConfig:......................{platform.PreferredPathWtqConfig}");

		Console.WriteLine($"PathToWtqConf:.................................{platform.PathToWtqConf}");
		Console.WriteLine($"PathToWtqConfDir:..............................{platform.PathToWtqConfDir}");

		Console.WriteLine($"Paths to WTQ configs");
		foreach (var p in platform.PathsToWtqConfs)
		{
			Console.WriteLine($" - {p}{(Fs.Inst.FileExists(p) ? " (exists)" : string.Empty)}");
		}

		Console.WriteLine($"PathToAppDir:..................................{platform.PathToAppDir}");
		Console.WriteLine($"PathToAssetsDir:...............................{platform.PathToAssetsDir}");
		Console.WriteLine($"PathToLogsDir:.................................{platform.PathToLogsDir}");
		Console.WriteLine($"PathToTempDir:.................................{platform.PathToTempDir}");
		Console.WriteLine($"PathToTrayIconDark:............................{platform.PathToTrayIconDark}");
		Console.WriteLine($"PathToTrayIconLight:...........................{platform.PathToTrayIconLight}");
		Console.WriteLine($"PathToUserHomeDir:.............................{platform.PathToUserHomeDir}");

		Console.WriteLine($"-----------------------------------------------------------------");
	}
}