using Microsoft.Extensions.DependencyInjection;
using Wtq.Services.SharpHook;

namespace Wtq.Services.Win32;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Configures SharpHook for handling global hotkeys.
	/// </summary>
	public static IServiceCollection AddSharpHookHotkeyService(this IServiceCollection services) =>
		Guard.Against.Null(services)
			.AddHostedService<SharpHookHotkeyService>();
}