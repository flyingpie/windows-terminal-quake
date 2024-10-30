using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.Win32;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWin32ProcessService(this IServiceCollection services)
	{
		return services
			.AddSingleton<IWtqWindowService, Win32WindowService>();
	}
}