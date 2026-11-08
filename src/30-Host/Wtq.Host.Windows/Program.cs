using Wtq.Host.Base;
using Wtq.Services.SharpHook;
using Wtq.Services.Win32;
using Wtq.Services.WinForms;

namespace Wtq.Host.Windows;

public static class Program
{
	[STAThread]
	public static void Main(string[] args) =>
		new WtqHost()
			.RunAsync(
				s =>
					s.AddSharpHookHotkeyService()
						.AddWin32Service()
						.AddWinFormsScreenInfoProvider()
						.AddWinFormsTrayIcon(),
				args
			)
			.GetAwaiter()
			.GetResult();
}
