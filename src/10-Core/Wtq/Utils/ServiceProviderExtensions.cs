using Microsoft.Extensions.DependencyInjection;
using Wtq.Services;

namespace Wtq.Utils;

public static class ServiceProviderExtensions
{
	public static IServiceCollection AddAsyncInitializable(this IServiceCollection services)
	{
		var a = services
			.Where(s => s.ImplementationType != null && s.ImplementationType.IsAssignableTo(typeof(IAsyncInitializable)));

		var d = new AsyncServiceInitializer();
		d.Services.AddRange(a);

		services.AddSingleton(d);

		return services;
	}

	public static async Task InitializeAsync(this IServiceProvider provider)
	{
		await provider.GetService<AsyncServiceInitializer>()!.InitializeAsync(provider).NoCtx();
	}
}