namespace Wtq.Host.Windows;

public static class Program
{
	public static async Task Main(string[] args)
	{
		Utils.Log.Configure();

		await new WtqWin32().RunAsync().ConfigureAwait(false);
	}
}