using Microsoft.Extensions.DependencyInjection;
using Wtq.Events;
using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddKWin(this IServiceCollection services)
	{
		return services
			.AddSingleton<KWinScriptExecutor>()
			.AddSingleton<IKWinClient>(p => new KWinClient(
				p.GetRequiredService<KWinScriptExecutor>(),
				p.GetRequiredService<KWinService>()))

			.AddSingleton<IDBusConnection, DBusConnection>()
			.AddSingleton(
				p =>
				{
					var dbus = p.GetRequiredService<IDBusConnection>();

					return new KWinService(dbus.ClientConnection, "org.kde.KWin");
				})
			.AddSingleton(
				p =>
				{
					var kwinService = p.GetRequiredService<KWinService>();

					return kwinService.CreateScripting("/Scripting");
				})
			.AddSingleton<Task<IWtqDBusObject>>(
				async p =>
				{
					var dbus = (DBusConnection)p.GetRequiredService<IDBusConnection>();
					await dbus.StartAsync(CancellationToken.None).NoCtx();

					var wtqDBusObj = new WtqDBusObject(p.GetRequiredService<IWtqBus>());

					await dbus.RegisterServiceAsync("wtq.svc", wtqDBusObj).ConfigureAwait(false);

					return wtqDBusObj;
				})
			.AddSingleton<KWinScriptExecutor>()
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