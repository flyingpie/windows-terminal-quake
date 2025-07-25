using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.Linux;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddKWin(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services;
	}
}