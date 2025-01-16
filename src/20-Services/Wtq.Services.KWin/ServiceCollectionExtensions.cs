using Microsoft.Extensions.DependencyInjection;
using Wtq.Configuration;
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

			.AddSingleton<IKWinScriptService, KWinScriptService>()
			.AddSingleton<IKWinClient, KWinClientV2>()

			.AddSingleton<IWtqWindowService, KWinWindowService>()
			.AddSingleton<IWtqScreenInfoProvider, KWinScreenInfoProvider>()

			.AddHostedService<KWinHotkeyService>();
	}
}