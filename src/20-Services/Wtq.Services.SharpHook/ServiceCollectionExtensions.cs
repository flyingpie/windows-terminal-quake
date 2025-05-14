using Microsoft.Extensions.DependencyInjection;
using Wtq.Services.SharpHook;

namespace Wtq.Services.Win32;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSharpHookHotkeyService(this IServiceCollection services)
	{
		return services
			.AddHostedService<SharpHookHotkeyService>();
	}
}