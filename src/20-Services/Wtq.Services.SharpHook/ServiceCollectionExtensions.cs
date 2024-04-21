using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.SharpHook;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSharpHookGlobalHotKeys(this IServiceCollection services)
	{
		return services
			.AddHostedService<SharpHookGlobalHotKeyService>();
	}
}