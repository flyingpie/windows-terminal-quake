using Microsoft.Extensions.DependencyInjection;
using Wtq.Host.Base;
using Wtq.Services.Win32;
using Wtq.Services.WinForms;

namespace Wtq.Host.Windows;

public class WtqWin32 : WtqHostBase
{
	protected override void ConfigureServices(IServiceCollection services)
	{
		Guard.Against.Null(services);

		services
			.AddWin32WindowService()
			.AddWinFormsHotkeyService()
			.AddWinFormsScreenInfoProvider()
			.AddWinFormsTrayIcon();
	}
}