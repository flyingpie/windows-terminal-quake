using Wtq.Services.Win32.Native;
using Wtq.Utils;

namespace Wtq.Host.Windows;

public static class Program
{
	[STAThread]
	public static async Task Main(string[] args)
	{
		if (Kernel32.AttachConsole(Kernel32.ATTACH_PARENT_PROCESS))
		{
			// We're now in CLI mode.
			// Note that the actual handling of CLI arguments is done in Wtq.Host.Base.
			Kernel32.RedirectConsoleStreams();
		}

		await new WtqWin32().RunAsync(args).NoCtx();
	}
}