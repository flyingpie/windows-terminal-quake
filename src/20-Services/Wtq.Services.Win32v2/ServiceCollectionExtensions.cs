using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.Win32v2;

public static class ServiceCollectionExtensions
{
	private static readonly ILogger Log = Utils.Log.For<Win32>();

	public static IServiceCollection AddWin32V2WindowService(this IServiceCollection services)
	{
		Log.LogInformation("Using Win32 v2 window service (new behavior, please report any issues and consider disabling this if you run into any)");

		return Guard.Against.Null(services)
			.AddSingleton<IWin32, Win32>()
			.AddSingleton<IWtqWindowService, Win32WindowService>();
	}
}