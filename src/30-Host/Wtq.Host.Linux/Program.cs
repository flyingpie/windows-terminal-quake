using Wtq.Services.KWin;

namespace Wtq.Host.Linux;

public static class Program
{
	public static async Task Main(string[] args)
	{
		Utils.Log.Configure();

		// await new KWinTest().TestAsync();

		// return;

		await new WtqLinux().RunAsync().ConfigureAwait(false);
	}
}