namespace Wtq.Host.Windows;

public static class Program
{
	[STAThread]
	public static async Task Main(string[] args)
	{
		Utils.Log.Configure();

		var app = new WtqWin32().RunAsync().ConfigureAwait(false);

		var ui = new WtqUI.WtqUI();

		ui.Start(args);

		await app;
	}
}