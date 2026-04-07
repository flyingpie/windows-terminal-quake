using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using Radzen;

namespace Wtq.Services.UI;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddUI(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddBlazorDesktop()
			.AddSingleton<WtqPhotinoBlazorApp>()
			.AddRadzenComponents()
			.AddTransient<Notifier>()
			.AddTransient<WtqUIHost>();
	}
}
