using Microsoft.Extensions.DependencyInjection;
using Wtq.Services;

namespace Wtq;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWtqCore(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services

			// Utils
			.AddSingleton<IWtqTween, WtqTween>()

			// Core App Logic
			.AddSingleton<IWtqAppToggleService, WtqAppToggleService>()
			.AddSingleton<IWtqBus, WtqBus>()
			.AddSingleton<IWtqWindowResolver, WtqWindowResolver>()
			.AddSingleton<IWtqAppRepo, WtqAppRepo>()

			.AddSingleton<WtqFocusTracker>()
			.AddSingleton<WtqHotkeyService>()
			.AddSingleton<WtqService>();
	}
}