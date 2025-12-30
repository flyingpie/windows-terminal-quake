using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wtq.Configuration;
using Wtq.Host.Base;
using Wtq.Services;
using Wtq.Services.Win32v2;
using Wtq.Services.WinForms;

namespace Wtq.Host.Windows;

public class WtqWin32 : WtqHostBase
{
	protected override IPlatformService CreatePlatformService() =>
		new WindowsPlatformService();

	protected override void ConfigureServices(IServiceCollection services)
	{
		Guard.Against.Null(services);

		var c = services.BuildServiceProvider().GetRequiredService<IOptions<WtqOptions>>().Value;

		services
			.AddHotkeyService(c)
			.AddWin32V2WindowService()
			.AddWinFormsScreenInfoProvider()

			// New cross-platform tray icon.
			//.AddTrayIcon()

			// WinForms (Windows-only) tray icon, kept around should the cross-platform one causes issues.
			.AddWinFormsTrayIcon()
			;
	}
}