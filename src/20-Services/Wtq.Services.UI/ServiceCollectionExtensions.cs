using Microsoft.Extensions.DependencyInjection;
using Radzen;
using System;

namespace Wtq.Services.UI;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddUI(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddRadzenComponents()
			.AddTransient<Notifier>()
			.AddHostedServiceSingleton<IWtqUIService, WtqUI>();
	}
}