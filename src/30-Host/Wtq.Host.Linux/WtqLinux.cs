using Microsoft.Extensions.DependencyInjection;
using Wtq.Services.KWin;

namespace Wtq.Host.Linux;

public class WtqLinux : WtqHostBase
{
	protected override void ConfigureServices(IServiceCollection services)
	{
		Guard.Against.Null(services);

		services
			.AddKWin();
	}
}