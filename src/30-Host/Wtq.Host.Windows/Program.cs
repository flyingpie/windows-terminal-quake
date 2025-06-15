using Wtq.Services.Win32.Native;

namespace Wtq.Host.Windows;

public static class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		if (Kernel32.AttachConsole(Kernel32.ATTACH_PARENT_PROCESS))
		{
			// We're now in CLI mode.
			// Note that the actual handling of CLI arguments is done in Wtq.Host.Base.
			Kernel32.RedirectConsoleStreams();
		}

		new WtqWin32().Run(args);
	}
}