using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.SharpHook;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Configures SharpHook for handling global hotkeys.
	/// </summary>
	public static IServiceCollection AddSharpHookHotkeyService(this IServiceCollection services) =>
		Guard.Against.Null(services)
			.AddHostedService<SharpHookHotkeyService>();
}