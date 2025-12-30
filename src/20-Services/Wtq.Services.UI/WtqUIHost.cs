using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Photino.Blazor;
using Photino.NET;
using Radzen;
using System.IO;

namespace Wtq.Services.UI;

public class WtqUIHost
{
	private readonly PhotinoBlazorApp2 _app;
	private const string MainWindowTitle = "WTQ - Main Window";

	private readonly ILogger _log = Log.For<WtqUIHost>();

	private readonly IPlatformService _platform;

	public WtqUIHost(IPlatformService platform, PhotinoBlazorApp2 app)
	{
		_app = app;
		_platform = Guard.Against.Null(platform);
	}

	public void Run()
	{
		// var app = new PhotinoBlazorApp2();
		// var appBuilder = PhotinoBlazorAppBuilder2.CreateDefault();
		// appBuilder.RootComponents.Add<App>("app");
		// appBuilder.Services
		// 	.AddConfiguration(_platform)
		// 	.AddRadzenComponents()
		// 	.AddSingleton(_platform)
		// 	.AddTransient<Notifier>()
		// 	.AddWtqCore()
		// 	;

		// services(appBuilder.Services);

		// var app = appBuilder.Build();
		_app.MainWindow
			.Center()
			.SetIconFile(Path.Combine(_platform.PathToAssetsDir, "icon-v2-256-padding.png"))
			.SetJavascriptClipboardAccessEnabled(true)
			.SetLogVerbosity(0)
			.SetSize(1270, 800)
			.SetTitle(MainWindowTitle);
		_app.Run();
	}
}

// public class PhotinoBlazorAppBuilder2
// {
// 	public PhotinoBlazorAppBuilder2()
// 	{
// 		this.RootComponents = new RootComponentList();
// 		this.Services = (IServiceCollection)new ServiceCollection();
// 	}
//
// 	public static PhotinoBlazorAppBuilder2 CreateDefault(string[] args = null)
// 	{
// 		return PhotinoBlazorAppBuilder2.CreateDefault((IFileProvider)null, args);
// 	}
//
// 	public static PhotinoBlazorAppBuilder2 CreateDefault(IFileProvider fileProvider, string[] args = null)
// 	{
// 		PhotinoBlazorAppBuilder2 blazorAppBuilder = new PhotinoBlazorAppBuilder2();
// 		// PhotinoBlazorAppBuilder blazorAppBuilder = Activator.CreateInstance<PhotinoBlazorAppBuilder>();
// 		// blazorAppBuilder.Services.AddBlazorDesktop(fileProvider);
// 		return blazorAppBuilder;
// 	}
//
// 	public RootComponentList RootComponents { get; }
//
// 	// public IServiceCollection Services { get; }
//
// 	public PhotinoBlazorApp2 Build(Action<IServiceProvider> serviceProviderOptions = null)
// 	{
// 		// ServiceProvider serviceProvider = this.Services.BuildServiceProvider();
// 		var rootComponents = new RootComponentList();
// 		PhotinoBlazorApp2 requiredService = serviceProvider.GetRequiredService<PhotinoBlazorApp2>();
// 		if (serviceProviderOptions != null)
// 			serviceProviderOptions((IServiceProvider)serviceProvider);
// 		requiredService.Initialize((IServiceProvider)serviceProvider, this.RootComponents);
// 		return requiredService;
// 	}
// }

public class PhotinoBlazorApp2
{
	public IServiceProvider Services { get; private set; }

	public BlazorWindowRootComponents RootComponents { get; private set; }

	public PhotinoBlazorApp2(IServiceProvider services)
	{
		var rootComponents = new RootComponentList();
		rootComponents.Add<App>("app");

		this.Services = services;
		this.RootComponents = this.Services.GetService<BlazorWindowRootComponents>();
		this.MainWindow = this.Services.GetService<PhotinoWindow>();
		this.WindowManager = this.Services.GetService<PhotinoWebViewManager>();
		this.MainWindow.SetTitle("Photino.Blazor App").SetUseOsDefaultSize(false).SetUseOsDefaultLocation(false).SetWidth(1000).SetHeight(900).SetLeft(450).SetTop(100);
		this.MainWindow.RegisterCustomSchemeHandler(PhotinoWebViewManager.BlazorAppScheme, new PhotinoWindow.NetCustomSchemeDelegate(this.HandleWebRequest));
		foreach ((Type, string) rootComponent in rootComponents)
			this.RootComponents.Add(rootComponent.Item1, rootComponent.Item2);
	}

	public PhotinoWindow MainWindow { get; private set; }

	public PhotinoWebViewManager WindowManager { get; private set; }

	public void Run()
	{
		if (string.IsNullOrWhiteSpace(this.MainWindow.StartUrl))
			this.MainWindow.StartUrl = "/";
		this.WindowManager.Navigate(this.MainWindow.StartUrl);
		this.MainWindow.WaitForClose();
	}

	public Stream HandleWebRequest(object sender, string scheme, string url, out string contentType)
	{
		return this.WindowManager.HandleWebRequest(sender, scheme, url, out contentType);
	}
}