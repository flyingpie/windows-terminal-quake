using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wtq.Configuration;
using Wtq.Host.Base;
using Wtq.Services.WinForms;

namespace Wtq.Host.Windows;

public class WtqWin32 : WtqHostBase
{
	protected override void ConfigureServices(IServiceCollection services)
	{
		Guard.Against.Null(services);

		var c = services.BuildServiceProvider().GetRequiredService<IOptions<WtqOptions>>().Value;

		services
			.AddHotkeyService(c)
			.AddWin32Service(c)
			.AddWinFormsScreenInfoProvider()

			// New cross-platform tray icon.
			//.AddTrayIcon()

			// WinForms (Windows-only) tray icon, kept around should the cross-platform one causes issues.
			.AddWinFormsTrayIcon()
			;
	}
}