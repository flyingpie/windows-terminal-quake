using Microsoft.Extensions.DependencyInjection;
using Wtq.Host.Base;
using Wtq.Services;
using Wtq.Services.KWin;
using Wtq.Services.Linux;
using Wtq.Services.TrayIcon;
using Wtq.Utils;

namespace Wtq.Host.Linux;

public class WtqLinux : WtqHostBase
{
	public static bool IsFlatpak =>
		EnvUtils.HasEnvVarWithValue(Os.WtqPlatformOverride, "flatpak") // For testing purposes.
		|| EnvUtils.HasEnvVarWithValue("container", "flatpak"); // Set by Flatpak.

	protected override IPlatformService CreatePlatformService() =>
		IsFlatpak
			? new LinuxFlatpakPlatformService()
			: new LinuxNativePlatformService();

	protected override void ConfigureServices(IServiceCollection services)
	{
		Guard.Against.Null(services);

		services
			.AddKWin()
			.AddTrayIcon();
	}
}