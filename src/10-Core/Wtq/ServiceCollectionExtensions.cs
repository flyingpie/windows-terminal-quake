using Microsoft.Extensions.DependencyInjection;
using Wtq.Services;

namespace Wtq;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddHostedServiceSingleton<TImplementation>(
		this IServiceCollection services)
		where TImplementation : class, IHostedService
	{
		Guard.Against.Null(services);

		return services
			.AddSingleton<TImplementation>()
			.AddHostedService<TImplementation>(p => (TImplementation)p.GetRequiredService<TImplementation>());
	}

	public static IServiceCollection AddHostedServiceSingleton<TService, TImplementation>(
		this IServiceCollection services)
		where TImplementation : class, IHostedService, TService
		where TService : class
	{
		Guard.Against.Null(services);

		return services
			.AddSingleton<TService, TImplementation>()
			.AddHostedService<TImplementation>(p => (TImplementation)p.GetRequiredService<TService>());
	}

	public static IServiceCollection AddWtqCore(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services

			// Utils
			.AddSingleton<IWtqTween, WtqTween>()

			// Core App Logic
			.AddSingleton<IWtqAppRepo, WtqAppRepo>()
			.AddSingleton<IWtqAppToggleService, WtqAppToggleService>()
			.AddSingleton<IWtqBus, WtqBus>()
			.AddSingleton<IWtqWindowResolver, WtqWindowResolver>()

			.AddHostedService<WtqFocusTracker>()
			.AddHostedService<WtqHotkeyService>()
			.AddHostedService<WtqService>();
	}
}