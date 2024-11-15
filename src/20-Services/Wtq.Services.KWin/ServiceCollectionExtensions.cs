using Microsoft.Extensions.DependencyInjection;
using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.Scripting;

namespace Wtq.Services.KWin;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddKWin(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services

			// DBus.
			.AddHostedServiceSingleton<IDBusConnection, DBusConnection>()
			.AddHostedServiceSingleton<IWtqDBusObject, WtqDBusObject>()
			.AddHostedServiceSingleton<IKWinClient, KWinClientV2>()

			.AddSingleton<IKWinScriptService, KWinScriptService>()
			.AddSingleton<IWtqWindowService, KWinWindowService>()
			.AddSingleton<IWtqScreenInfoProvider, KWinScreenInfoProvider>()

			.AddHostedService<KWinHotkeyService>()
			.AddHostedService<KWinTrayIconService>();
	}
}