namespace Wtq.Host.Linux;

public static class Program
{
	public static async Task Main(string[] args)
	{
		Utils.Log.Configure();

		await new WtqLinux().RunAsync().ConfigureAwait(false);
	}
}