using Microsoft.Extensions.DependencyInjection;

namespace Wtq;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWtqCore(this IServiceCollection services)
	{
		return services;
	}
}