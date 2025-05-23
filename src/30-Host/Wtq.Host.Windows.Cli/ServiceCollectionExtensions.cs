using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wtq.Configuration;
using Wtq.Services.Win32;
using Wtq.Services.Win32v2;
using Wtq.Services.WinForms;
using Wtq.Utils;

namespace Wtq.Host.Windows.Cli;

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
			_log.LogInformation("Using SharpHook hotkey service");

			services.AddSharpHookHotkeyService();
		}
		else
		{
			_log.LogInformation("Using WinForms hotkey service");

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
			_log.LogInformation("Using Win32 v1 window service");

			services.AddWin32WindowService();
		}
		else
		{
			_log.LogInformation("Using Win32 v2 window service");

			services.AddWin32V2WindowService();
		}

		return services;
	}
}