using Microsoft.Extensions.DependencyInjection;
using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddKWin(this IServiceCollection services)
	{
		return services

			// DBus.
			.AddSingleton<IDBusConnection, DBusConnection>()

			// .AddSingleton<DBus.KWinService>(
			// 	p => new KWinService(p.GetRequiredService<IDBusConnection>().ClientConnection, "org.kde.KWin"))
			//
			// .AddSingleton<DBus.KWin>(
			// 	p => p.GetRequiredService<KWinService>().CreateKWin("/KWin"))
			//
			// .AddSingleton<DBus.Scripting>(
			// 	p => p.GetRequiredService<KWinService>().CreateScripting("/Scripting"))

			.AddSingleton<IWtqDBusObject, WtqDBusObject>()

			.AddSingleton<IKWinScriptService, KWinScriptService>()
			.AddSingleton<IKWinClient, KWinClientV2>()

			.AddKWinProcessService()
			.AddKWinScreenCoordsProvider()

			.AddHostedService<KWinHotKeyService>();
	}

	public static IServiceCollection AddKWinProcessService(this IServiceCollection services)
	{
		return services
			.AddSingleton<IWtqProcessService, KWinProcessService>();
	}

	public static IServiceCollection AddKWinScreenCoordsProvider(this IServiceCollection services)
	{
		return services
			.AddSingleton<IWtqScreenInfoProvider, KWinScreenInfoProvider>();
	}
}