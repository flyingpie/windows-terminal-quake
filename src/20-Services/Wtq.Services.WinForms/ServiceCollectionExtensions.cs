using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.WinForms;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWinFormsHotkeyService(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddSingleton<WinFormsHotkeyService>();
	}

	public static IServiceCollection AddWinFormsScreenInfoProvider(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddSingleton<IWtqScreenInfoProvider, WinFormsScreenInfoProvider>();
	}

	public static IServiceCollection AddWinFormsTrayIcon(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddHostedService<WinFormsTrayIconService>();
	}
}