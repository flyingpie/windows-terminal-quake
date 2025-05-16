using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wtq.Configuration;
using Wtq.Host.Base;
using Wtq.Services.Win32;
using Wtq.Services.WinForms;
using Wtq.Utils;

namespace Wtq.Host.Windows.Cli;

public class WtqWin32 : WtqHostBase
{
	protected override void ConfigureServices(IServiceCollection services)
	{
		Guard.Against.Null(services);

		var log = Log.For<WtqWin32>();

		var c = services.BuildServiceProvider().GetRequiredService<IOptions<WtqOptions>>().Value;

		if (c.FeatureFlags.SharpHook)
		{
			log.LogInformation("Using SharpHook hotkey service");

			services.AddSharpHookHotkeyService();
		}
		else
		{
			log.LogInformation("Using WinForms hotkey service");

			services.AddWinFormsHotkeyService();
		}

		services
			.AddWin32WindowService()

			.AddWinFormsScreenInfoProvider()

			// New cross-platform tray icon.
			//.AddTrayIcon()

			// WinForms (Windows-only) tray icon, kept around should the cross-platform one causes issues.
			.AddWinFormsTrayIcon()
		;
	}
}