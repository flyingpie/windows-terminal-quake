using System.IO;

namespace Wtq.Services.UI;

public class WtqUIHost(IPlatformService platform, WtqPhotinoBlazorApp app)
{
	private readonly IPlatformService _platform = Guard.Against.Null(platform);

	public void Run()
	{
		app.MainWindow
			//
			.Center()
			.SetIconFile(Path.Combine(_platform.PathToAssetsDir, "icon-v2-256-padding.png"))
			.SetJavascriptClipboardAccessEnabled(true)
			.SetLogVerbosity(0)
			.SetSize(1270, 800)
			.SetTitle("WTQ - Main Window");

		app.Run();
	}
}