using Wtq.Utils;

namespace Wtq.Host.Windows;

public static class Program
{
	[STAThread]
	public static async Task Main(string[] args)
	{
		Log.Configure();

		await new WtqWin32().RunAsync().NoCtx();
	}
}