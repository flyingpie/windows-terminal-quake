using Microsoft.Extensions.DependencyInjection;
using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddKWin(this IServiceCollection services)
	{
		return services
			.AddSingleton<IKWinClient>(p => new KWinClient(p.GetRequiredService<KWinScriptExecutor>()))
			.AddSingleton<IDBusConnection, DBusConnection>()
			.AddSingleton<KWinService>(
				p =>
				{
					var dbus = p.GetRequiredService<IDBusConnection>();

					return new KWinService(dbus.ClientConnection, "org.kde.KWin");
				})
			.AddSingleton<Scripting>(
				p =>
				{
					var kwinService = p.GetRequiredService<KWinService>();

					return kwinService.CreateScripting("/Scripting");
				})
			.AddSingleton<Task<IWtqDBusObject>>(
				async p =>
				{
					var dbus = (DBusConnection)p.GetRequiredService<IDBusConnection>();
					await dbus.StartAsync(CancellationToken.None);

					var wtqDBusObj = new WtqDBusObject();

					await dbus.RegisterServiceAsync("wtq.svc", wtqDBusObj).ConfigureAwait(false);

					return wtqDBusObj;
				})
			.AddSingleton<KWinScriptExecutor>()
			.AddKWinProcessService()
			.AddKWinScreenCoordsProvider();
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