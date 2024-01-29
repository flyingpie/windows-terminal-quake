using Microsoft.Extensions.DependencyInjection;

namespace Wtq.SharpHook;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSharpHookGlobalHotkeys(this IServiceCollection services)
	{
		return services
			.AddHostedService<SharpHookGlobalHotkeyService>();
	}
}