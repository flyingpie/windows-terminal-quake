using Microsoft.Extensions.DependencyInjection;

namespace Wtq.SimpleTrayIcon;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSimpleTrayIcon(this IServiceCollection services)
	{
		return services
			.AddHostedService<SimpleTrayIconService>();
	}
}