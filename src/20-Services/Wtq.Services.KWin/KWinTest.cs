using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wtq.Data;
using Wtq.Services.KWin.DBus;
using Address = Tmds.DBus.Protocol.Address;
using Connection = Tmds.DBus.Protocol.Connection;

namespace Wtq.Services.KWin;

public class KWinTest
{
	public async Task TestAsync()
	{
		var p = new ServiceCollection()
			.AddKWin()
			.BuildServiceProvider();

		// var dbus = p.GetRequiredService<IDBusConnection>() as IHostedService;
		// await dbus.StartAsync(CancellationToken.None);

		//
		var kwin = p.GetRequiredService<IKWinClient>();

		var hk = p.GetRequiredService<KWinHotKeyService>();
		await hk.StartAsync(CancellationToken.None);

		var clients = await kwin.GetClientListAsync(CancellationToken.None).ConfigureAwait(false);

		var wz = clients.FirstOrDefault(x => x.ResourceClass.Contains("wezterm", StringComparison.OrdinalIgnoreCase));

		await kwin
			.MoveClientAsync(
				wz,
				new WtqRect()
				{
					X = 50, Y = 50, Width = 1800, Height = 800,
				},
				CancellationToken.None)
			.ConfigureAwait(false);

		var dbg = 2;
	}
}