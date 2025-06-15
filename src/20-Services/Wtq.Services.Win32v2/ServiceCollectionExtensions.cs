using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.Win32v2;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWin32V2WindowService(this IServiceCollection services)
	{
		return Guard.Against.Null(services)
			.AddSingleton<IWin32, Win32>()
			.AddSingleton<IWtqWindowService, Win32WindowService>();
	}
}