using Microsoft.Extensions.DependencyInjection;
using Wtq.Host.Base;
using Wtq.Services;
using Wtq.Services.SharpHook;
using Wtq.Services.Win32;
using Wtq.Services.WinForms;

namespace Wtq.Host.Windows;

public class WtqWin32 : WtqHostBase
{
	protected override IPlatformService CreatePlatformService() => new WindowsPlatformService();

	protected override void ConfigureServices(IServiceCollection services)
	{
		Guard.Against.Null(services);

		services
			.AddSharpHookHotkeyService()
			.AddWin32Service()
			.AddWinFormsScreenInfoProvider()
			// New cross-platform tray icon.
			//.AddTrayIcon()

			// WinForms (Windows-only) tray icon, kept around should the cross-platform one causes issues.
			.AddWinFormsTrayIcon();
	}
}
