using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.Stubs;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddStubs(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddSingleton<IWtqWindowService, StubWtqWindowService>()
			.AddSingleton<IWtqScreenInfoProvider, StubWtqScreenInfoProvider>();
	}
}