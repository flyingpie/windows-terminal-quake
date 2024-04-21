using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.SimpleTrayIcon;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSimpleTrayIcon(this IServiceCollection services)
	{
		return services
			.AddHostedService<SimpleTrayIconService>();
	}
}