using System.Runtime.InteropServices;

namespace Wtq.Utils;

public static class Os
{
	public const string WtqPlatformOverride = "WTQ_PLATFORM_OVERRIDE";

	public static bool IsLinux =>
		EnvUtils.HasEnvVarWithValue(WtqPlatformOverride, "linux") // For testing purposes.
		|| RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

	public static bool IsWindows =>
		EnvUtils.HasEnvVarWithValue(WtqPlatformOverride, "windows") // For testing purposes.
		|| RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}