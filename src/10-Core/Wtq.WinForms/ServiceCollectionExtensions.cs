using Microsoft.Extensions.DependencyInjection;
using Wtq.Core.Services;
using Wtq.Win32;

namespace Wtq.WinFomsrms;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWinFormsHotkeyService(this IServiceCollection services)
	{
		return services
			.AddHostedService<WinFormsHotkeyService>();
	}

	public static IServiceCollection AddWinFormsScreenCoordsProvider(this IServiceCollection services)
	{
		return services
			.AddSingleton<IWtqScreenCoordsProvider, WinFormsScreenCoordsProvider>();
	}
}