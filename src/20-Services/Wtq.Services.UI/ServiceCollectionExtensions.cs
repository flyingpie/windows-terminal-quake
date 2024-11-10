using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace Wtq.Services.UI;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddUI(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddRadzenComponents()
			.AddSingleton<WtqUI>()
			.AddSingleton<IWtqUIService>(p => p.GetRequiredService<WtqUI>());
	}
}