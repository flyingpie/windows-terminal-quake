using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Wtq;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSingletonHostedService<TService, TImplementation>(this IServiceCollection services)
		where TService : class
		where TImplementation : class, TService, IHostedService
	{
		Guard.Against.Null(services);

		services.AddSingleton<TService, TImplementation>();

		services.AddHostedService(p => (TImplementation)p.GetRequiredService<TService>());

		return services;
	}

	public static IServiceCollection AddWtqCore(this IServiceCollection services)
	{
		return services;
	}
}