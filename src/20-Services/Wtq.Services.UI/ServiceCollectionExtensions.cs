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
			.AddSingleton<PhotinoBlazorApp2>()
			.AddRadzenComponents()
			.AddTransient<Notifier>()
			.AddTransient<WtqUIHost>();
	}
}