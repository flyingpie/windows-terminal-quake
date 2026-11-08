using System.Runtime.InteropServices;
using Wtq.Services;
using Wtq.Services.Linux;
using Wtq.Services.Win32;

namespace Wtq.Host.Base;

public static class PlatformServiceFactory
{
	public static IPlatformService Create()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return new WindowsPlatformService();
		}

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			var isFlatpak =
				EnvUtils.HasEnvVarWithValue(Os.WtqPlatformOverride, "flatpak") // For testing purposes.
				|| EnvUtils.HasEnvVarWithValue("container", "flatpak"); // Set by Flatpak.

			return isFlatpak ? new LinuxFlatpakPlatformService() : new LinuxNativePlatformService();
		}

		throw new InvalidOperationException("Unknown OS platform");
	}
}
