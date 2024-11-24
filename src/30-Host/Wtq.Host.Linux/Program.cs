namespace Wtq.Host.Linux;

public static class Program
{
	public static async Task Main(string[] args)
	{
		await new WtqLinux().RunAsync(args).NoCtx();
	}
}