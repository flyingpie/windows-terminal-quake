using Microsoft.Extensions.DependencyInjection;
using Wtq.Host.Base;
using Wtq.Services;
using Wtq.Services.KWin;
using Wtq.Services.TrayIcon;

namespace Wtq.Host.Linux;

public class WtqLinux : WtqHostBase
{
	protected override IPlatformService CreatePlatformService()
	{
		return new LinuxFlatpakPlatformService();
	}

	protected override void ConfigureServices(IServiceCollection services)
	{
		Guard.Against.Null(services);

		services
			.AddTrayIcon()
			.AddKWin();
	}
}