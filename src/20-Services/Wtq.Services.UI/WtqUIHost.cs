using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using Photino.NET;

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

public class WtqPhotinoBlazorApp
{
	public WtqPhotinoBlazorApp(IServiceProvider services)
	{
		var rootComponents = new RootComponentList();
		rootComponents.Add<App>("app");

		Services = services;
		RootComponents = Services.GetService<BlazorWindowRootComponents>();
		MainWindow = Services.GetService<PhotinoWindow>();
		WindowManager = Services.GetService<PhotinoWebViewManager>();
		MainWindow
			.SetTitle("Photino.Blazor App")
			.SetUseOsDefaultSize(false)
			.SetUseOsDefaultLocation(false)
			.SetWidth(1000)
			.SetHeight(900)
			.SetLeft(450)
			.SetTop(100);

		MainWindow.RegisterCustomSchemeHandler(
			PhotinoWebViewManager.BlazorAppScheme,
			new(HandleWebRequest)
		);
		foreach ((Type, string) rootComponent in rootComponents)
		{
			RootComponents.Add(rootComponent.Item1, rootComponent.Item2);
		}
	}

	public IServiceProvider Services { get; }

	public BlazorWindowRootComponents RootComponents { get; }

	public PhotinoWindow MainWindow { get; }

	public PhotinoWebViewManager WindowManager { get; }

	public void Run()
	{
		if (string.IsNullOrWhiteSpace(MainWindow.StartUrl))
		{
			MainWindow.StartUrl = "/";
		}

		WindowManager.Navigate(MainWindow.StartUrl);
		MainWindow.WaitForClose();
	}

	private Stream HandleWebRequest(
		object sender,
		string scheme,
		string url,
		out string contentType
	)
	{
		return WindowManager.HandleWebRequest(sender, scheme, url, out contentType);
	}
}
