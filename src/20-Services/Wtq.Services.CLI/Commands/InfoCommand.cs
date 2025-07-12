namespace Wtq.Services.CLI.Commands;

[Command]
public class InfoCommand : ICommand
{
	public void Execute()
	{
		Console.WriteLine($"-- Paths --------------------------------------------------------");
		// Console.WriteLine($"{nameof(WtqPaths.AppData)}:....................{WtqPaths.AppData}");
		// Console.WriteLine($"{nameof(WtqPaths.AppDataWtq)}:.................{WtqPaths.AppDataWtq}");
		// Console.WriteLine($"{nameof(WtqPaths.PreferredWtqConfigPath)}:.....{WtqPaths.PreferredWtqConfigPath}");
		// Console.WriteLine($"{nameof(WtqPaths.UserHome)}:...................{WtqPaths.UserHome}");
		// Console.WriteLine($"{nameof(WtqPaths.WtqAppDir)}:..................{WtqPaths.WtqAppDir}");
		// Console.WriteLine($"{nameof(WtqPaths.WtqConfigFromEnvVar)}:........{WtqPaths.WtqConfigFromEnvVar}");
		// Console.WriteLine($"{nameof(WtqPaths.WtqConfigPaths)}:.............{WtqPaths.WtqConfigPaths}");
		// Console.WriteLine($"{nameof(WtqPaths.WtqLogDir)}:..................{WtqPaths.WtqLogDir}");
		// Console.WriteLine($"{nameof(WtqPaths.WtqTempDir)}:.................{WtqPaths.WtqTempDir}");
		Console.WriteLine($"-- /Paths -------------------------------------------------------");
	}
}