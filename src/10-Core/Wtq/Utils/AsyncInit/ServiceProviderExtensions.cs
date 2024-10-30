using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Utils.AsyncInit;

public static class ServiceProviderExtensions
{
	/// <summary>
	/// Adds <see cref="AsyncServiceInitializer"/> to the service collection.<br/>
	/// Make sure to call this _after_ all the other services are registered, so it can find all relevant types.
	/// </summary>
	public static IServiceCollection AddAsyncInitializable(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		// Fetch all services that are flagged with IAsyncInitializable.
		var flaggedServices = services

			// Only singletons are supported for now.
			.Where(s => s.Lifetime == ServiceLifetime.Singleton)

			// Look for flagged types.
			.Where(s => s.ImplementationType != null && s.ImplementationType.IsAssignableTo(typeof(IAsyncInitializable)));

		// Add found services to service initializer.
		var d = new AsyncServiceInitializer();
		d.Services.AddRange(flaggedServices);

		return services
			.AddSingleton(d);
	}

	public static async Task InitializeAsync(this IServiceProvider provider)
	{
		Guard.Against.Null(provider);

		// Execute async initialize on flagged services.
		await provider
			.GetRequiredService<AsyncServiceInitializer>()
			.InitializeAsync(provider)
			.NoCtx();
	}
}