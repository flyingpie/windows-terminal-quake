namespace Wtq.Host.Windows;

public static class Program
{
	public static async Task Main(string[] args)
	{
		await new WtqWin32().RunAsync(args).NoCtx();
	}
}