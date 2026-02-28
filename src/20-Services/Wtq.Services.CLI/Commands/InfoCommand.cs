using Microsoft.Extensions.Options;
using Wtq.Configuration;

namespace Wtq.Services.CLI.Commands;

[Command(Parent = typeof(AppRootCommand))]
public class InfoCommand(IOptions<WtqOptions> opts, IPlatformService platform) : ICommand
{
	[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "MvdO: So the strings line up nicely.")]
	public void Execute()
	{
		Console.WriteLine($"-- App ----------------------------------------------------------");
		Console.WriteLine($"Platform:......................................{platform.PlatformName}");
		Console.WriteLine($"App version:...................................{WtqConstants.AppVersion}");
		Console.WriteLine($"File version:..................................{WtqConstants.AppFileVersion}");
		Console.WriteLine($"Informational version:.........................{WtqConstants.AppInformationalVersion}");
		Console.WriteLine($"Build configuration:...........................{WtqConstants.BuildConfiguration}");
		Console.WriteLine($"Build date:....................................{WtqConstants.BuildDate.ToIso8601()}");
		Console.WriteLine($"Git branch:....................................{WtqConstants.GitBranch}");
		Console.WriteLine($"Git commit:....................................{WtqConstants.GitCommit}");
		Console.WriteLine($"OS color mode:.................................{platform.OsColorMode}");
		Console.WriteLine($"-- API ----------------------------------------------------------");
		Console.WriteLine($"API enabled:...................................{opts.Value.Api?.Enable ?? false}");
		Console.WriteLine($"API endpoints:.................................{string.Join(", ", opts.Value.Api?.Urls ?? platform.DefaultApiUrls)}");

		Console.WriteLine($"-- Config paths -------------------------------------------------");
		Console.WriteLine($"PreferredPathToWtqConfig:......................{platform.PreferredPathWtqConfig}");
		Console.WriteLine($"PathToWtqConf:.................................{platform.PathToWtqConf}");
		Console.WriteLine($"PathToWtqConfDir:..............................{platform.PathToWtqConfDir}");
		Console.WriteLine($"Paths to WTQ configs:..........................{platform.PathsToWtqConfs.First()}");
		foreach (var p in platform.PathsToWtqConfs.Skip(1))
		{
			Console.WriteLine($"...............................................{p}{(Fs.Inst.FileExists(p) ? " (exists)" : string.Empty)}");
		}

		Console.WriteLine($"-- Misc paths ---------------------------------------------------");
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