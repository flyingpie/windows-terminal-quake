using Microsoft.Extensions.DependencyInjection;

namespace Wtq.SharpHook;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSharpHookGlobalHotKeys(this IServiceCollection services)
	{
		return services
			.AddHostedService<SharpHookGlobalHotKeyService>();
	}
}