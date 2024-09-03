using Wtq.Utils;

namespace Wtq.Host.Linux;

public static class Program
{
	public static async Task Main(string[] args)
	{
		Log.Configure();

		await new WtqLinux().RunAsync().NoCtx();
	}
}