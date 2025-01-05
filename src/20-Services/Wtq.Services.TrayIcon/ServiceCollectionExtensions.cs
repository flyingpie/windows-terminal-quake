using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.TrayIcon;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddTrayIcon(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddHostedService<WtqTrayIconService>();
	}
}