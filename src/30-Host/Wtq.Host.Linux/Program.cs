using Wtq.Services.KWin.DBus;

namespace Wtq.Host.Linux;

public static class Program
{
	public static async Task Main(string[] args)
	{
		await new KWinClient1().StuffAsync();

		return;
		Utils.Log.Configure();

		await new WtqLinux().RunAsync().ConfigureAwait(false);
	}
}