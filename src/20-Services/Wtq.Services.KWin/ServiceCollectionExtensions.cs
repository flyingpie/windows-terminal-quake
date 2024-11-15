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
			.AddSingleton<IDBusConnection, DBusConnection>()
			.AddSingleton<IWtqDBusObject, WtqDBusObject>()
			.AddSingleton<IKWinClient, KWinClientV2>()
			.AddSingleton<IKWinScriptService, KWinScriptService>()

			.AddSingleton<IWtqScreenInfoProvider, KWinScreenInfoProvider>()
			.AddSingleton<IWtqWindowService, KWinWindowService>()

			.AddHostedService<KWinHotkeyService>()
			.AddHostedService<KWinTrayIconService>();
	}
}