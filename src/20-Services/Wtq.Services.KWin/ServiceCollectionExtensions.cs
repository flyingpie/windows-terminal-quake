using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.KWin;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddKWin(this IServiceCollection services)
	{
		return services
			.AddSingleton<IWtqProcessService, KWinProcessService>()

			.AddKWinProcessService()
			.AddKWinScreenCoordsProvider();
	}

	public static IServiceCollection AddKWinProcessService(this IServiceCollection services)
	{
		return services
			.AddSingleton<IWtqProcessService, KWinProcessService>();
	}

	public static IServiceCollection AddKWinScreenCoordsProvider(this IServiceCollection services)
	{
		return services
			.AddSingleton<IWtqScreenInfoProvider, KWinScreenInfoProvider>();
	}
}