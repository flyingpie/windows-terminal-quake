using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.Win32;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWin32WindowService(this IServiceCollection services)
	{
		return Guard.Against.Null(services)
			.AddSingleton<IWin32, Win32>()
			.AddSingleton<IWtqWindowService, Win32WindowService>();
	}
}