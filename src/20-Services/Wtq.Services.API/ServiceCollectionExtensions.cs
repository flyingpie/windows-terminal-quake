using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.API;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApi(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddHostedService<ApiService>();
	}
}