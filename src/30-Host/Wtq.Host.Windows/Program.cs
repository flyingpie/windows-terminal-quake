using Wtq.Services.Win32;

namespace Wtq.Host.Windows;

public static class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		if (Win32.AttachConsole())
		{
			// We're now in CLI mode.
			// Note that the actual handling of CLI arguments is done in Wtq.Host.Base.
			Win32.RedirectConsoleStreams();
		}

		new WtqWin32().RunAsync(args).GetAwaiter().GetResult();
	}
}