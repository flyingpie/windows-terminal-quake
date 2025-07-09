using Microsoft.Extensions.DependencyInjection;
using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.ProcessFactory;
using Wtq.Services.KWin.Scripting;

namespace Wtq.Services.KWin;

public static class ServiceCollectionExtensions
{
	private static readonly ILogger _log = Log.For(typeof(ServiceCollectionExtensions));

	public static IServiceCollection AddKWin(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		if (Os.IsFlatpak)
		{
			_log.LogDebug("Using Flatpak process factory");
			services.AddSingleton<IProcessFactory, FlatpakProcessFactory>();
		}
		else
		{
			_log.LogDebug("Using native process factory");
			services.AddSingleton<IProcessFactory, NativeProcessFactory>();
		}

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