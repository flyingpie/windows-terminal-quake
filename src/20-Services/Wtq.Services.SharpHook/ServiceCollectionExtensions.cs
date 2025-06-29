using Microsoft.Extensions.DependencyInjection;
using Wtq.Services.SharpHook.Input;

namespace Wtq.Services.SharpHook;

public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Configures SharpHook for handling global hotkeys.
	/// </summary>
	public static IServiceCollection AddSharpHookHotkeyService(this IServiceCollection services) =>
		Guard.Against.Null(services)
			.AddSingleton<IWin32, Win32>()
			.AddSingleton<IWin32KeyService, Win32KeyService>()
			.AddHostedService<SharpHookHotkeyService>();
}