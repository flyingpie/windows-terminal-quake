using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Core;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWtqCore(this IServiceCollection services)
	{
		return services;
	}
}