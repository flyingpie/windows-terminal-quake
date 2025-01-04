using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Photino.Blazor;
using System.Collections.Generic;
using System.Drawing;

namespace Wtq.Services.UI;

public sealed class WtqUI
{
	private readonly IWtqWindowService _windowService;

	public WtqUI(IWtqWindowService windowService)
	{
		_windowService = Guard.Against.Null(windowService);
	}

	public static void Run(Action<IServiceCollection> services)
	{
		Guard.Against.Null(services);

		var log = Log.For<WtqUI>();
		log.LogDebug("UI thread starting");

		var appBuilder = PhotinoBlazorAppBuilder.CreateDefault();

		using var invoker = new WtqUIInvoker();

		// TODO: Unify with the main app DI.
		appBuilder.Services
			.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>()
			.AddSingleton<IWtqUIService>(_ => invoker);

		services(appBuilder.Services);

		appBuilder.RootComponents.Add<App>("app");

		var app = appBuilder.Build();

		invoker.Action = a => app?.MainWindow?.Invoke(a);

		_ = new WtqUIHost(
			app.Services.GetRequiredService<IWtqBus>(),
			app.Services.GetRequiredService<IWtqWindowService>(),
			app.Services.GetRequiredService<IWtqScreenInfoProvider>(),
			app.Services.GetRequiredService<IHostApplicationLifetime>(),
			app.Services.GetRequiredService<IEnumerable<IHostedService>>(),
			app);

		((ApplicationLifetime)app.Services.GetRequiredService<IHostApplicationLifetime>()).NotifyStarted();

		app.Run();

		log.LogDebug("UI thread exiting");
	}
}