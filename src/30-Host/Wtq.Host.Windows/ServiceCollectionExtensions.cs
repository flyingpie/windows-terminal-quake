using Microsoft.Extensions.DependencyInjection;
using Wtq.Configuration;
using Wtq.Services.SharpHook;

namespace Wtq.Host.Windows;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddHotkeyService(
		this IServiceCollection services,
		WtqOptions opts
	)
	{
		Guard.Against.Null(services);
		Guard.Against.Null(opts);

		return services.AddSharpHookHotkeyService();
	}
}
