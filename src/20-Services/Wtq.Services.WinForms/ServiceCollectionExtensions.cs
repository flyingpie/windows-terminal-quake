using Microsoft.Extensions.DependencyInjection;
using Wtq.Services.Win32;

namespace Wtq.Services.WinForms;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWinFormsHotKeyService(this IServiceCollection services)
	{
		return services
			.AddHostedService<WinFormsHotKeyService>();
	}

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