using Wtq.Host.Base;
using Wtq.Services.KWin;
using Wtq.Services.TrayIcon;

namespace Wtq.Host.Linux;

public static class Program
{
	public static Task Main(string[] args) =>
		new WtqHost().RunAsync(s => s.AddKWin().AddTrayIcon(), args);
}
