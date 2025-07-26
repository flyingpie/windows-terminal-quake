using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.Linux;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddLinux(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services;
	}
}