using System.Runtime.InteropServices;

namespace Wtq;

public static class WtqEnv
{
	public static class Names
	{
		public const string Config = "WTQ_CONFIG_FILE";
	}

	/// <summary>
	/// Returns path to the WTQ config file, as specified by an environment variable.
	/// </summary>
	public static string? ConfigFile
		=> Environment.GetEnvironmentVariable(Names.Config)?.ExpandEnvVars()?.EmptyOrWhiteSpaceToNull();

	public static bool IsLinux
		=> RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

	public static bool IsWindows
		=> RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

	public static bool HasTermEnvVar
		=> !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TERM"));
}