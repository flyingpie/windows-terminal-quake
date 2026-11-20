using Microsoft.Extensions.DependencyInjection;
using Tmds.DBus.Protocol;
using Wtq.Services;
using Wtq.Services.KWin;
using Wtq.Services.KWin.DBus;
using Wtq.Services.Linux;

namespace Wtq.E2ETest;

[TestClass]
public class Test1
{
	[TestInitialize]
	public async Task SetupAsync()
	{
		// // Opted in at this point (see E2ECondition), so fail loudly if the environment isn't usable.
		// if (Process.GetProcessesByName("wtq").Length > 0)
		// {
		// 	Assert.Fail("Another WTQ instance is running, please close it first.");
		// }
		//
		// if (!File.Exists(WtqProcess.PathToExe))
		// {
		// 	Assert.Fail($"WTQ executable not found at '{WtqProcess.PathToExe}'.");
		// }

		// await _kwin.ConnectAsync();

		// Clean up leftovers from earlier (crashed) runs.
		// await CloseTestWindowsAsync();
	}

	[TestMethod]
	public async Task METHOD()
	{
		var s = new ServiceCollection()
			//
			.AddSingleton<IPlatformService, LinuxNativePlatformService>()
			.AddSingleton<IWtqBus, WtqBus>()
			.AddKWin()
			.BuildServiceProvider();

		var kwin = s.GetRequiredService<IKWinClient>();

		var x = await kwin.GetWindowListAsync(default);

		var zz = 0;

		var WtqJsonc =
				// language=json
				$$"""
				{
					"Apps": [
						{
							"Name": "konsole",
							"FileName": "konsole",
							"Hotkeys": [{ "Modifiers": "Control", "Key": "F12" }],
							"AttachMode": "FindOrStart",
							"TaskbarIconVisibility": "AlwaysVisible"
						}
					]
				}
				""";
	}
}
