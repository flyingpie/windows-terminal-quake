using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wtq.Configuration;
using Wtq.Services.Win32;
using Wtq.Services.Win32v2;
using Wtq.Services.WinForms;
using Wtq.Utils;

namespace Wtq.Host.Windows;

public static class ServiceCollectionExtensions
{
	private static readonly ILogger _log = Log.For<WtqWin32>();

	public static IServiceCollection AddHotkeyService(
		this IServiceCollection services,
		WtqOptions opts)
	{
		Guard.Against.Null(services);
		Guard.Against.Null(opts);

		if (opts.FeatureFlags?.SharpHook ?? false)
		{
			_log.LogInformation("Using SharpHook hotkey service (new behavior, please report any issues and consider disabling this if you run into any)");

			services.AddSharpHookHotkeyService();
		}
		else
		{
			_log.LogInformation("Using WinForms hotkey service (a feature flag is available, which enables using the 'Windows' modifier)");

			services.AddWinFormsHotkeyService();
		}

		return services;
	}

	public static IServiceCollection AddWin32Service(
		this IServiceCollection services,
		WtqOptions opts)
	{
		Guard.Against.Null(services);
		Guard.Against.Null(opts);

		if (opts.FeatureFlags?.NewWindowCapture ?? false)
		{
			_log.LogInformation("Using Win32 v2 window service (new behavior, please report any issues and consider disabling this if you run into any)");

			services.AddWin32V2WindowService();
		}
		else
		{
			_log.LogInformation("Using Win32 v1 window service (a feature flag is available, which enables attaching to more window types)");

			services.AddWin32WindowService();
		}

		return services;
	}
}