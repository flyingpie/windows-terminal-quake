using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.WinForms;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWinFormsScreenInfoProvider(this IServiceCollection services)
	{
		return services
			.AddSingleton<IWtqScreenInfoProvider, WinFormsScreenInfoProvider>();
	}

	public static IServiceCollection AddWinFormsTrayIcon(this IServiceCollection services)
	{
		return services
			.AddHostedService<WinFormsTrayIconService>();
	}
}