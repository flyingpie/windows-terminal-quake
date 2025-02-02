using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Photino.Blazor;

namespace Wtq.Services.UI;

public static class WtqUIHostBuilder
{
	public static void Run(Action<IServiceCollection> services)
	{
		Guard.Against.Null(services);

		var appBuilder = PhotinoBlazorAppBuilder.CreateDefault();

		using var invoker = new WtqUIInvoker();

		appBuilder.Services
			.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>()
			.AddSingleton<IWtqUIService>(_ => invoker);

		services(appBuilder.Services);

		appBuilder.RootComponents.Add<App>("app");

		var app = appBuilder.Build();

		invoker.Action = a => app?.MainWindow?.Invoke(a);

		new WtqUIHost(
			app.Services.GetRequiredService<IEnumerable<IHostedService>>(),
			app.Services.GetRequiredService<IHostApplicationLifetime>(),
			app.Services.GetRequiredService<IWtqBus>(),
			app.Services.GetRequiredService<IWtqWindowService>(),
			app);

		((ApplicationLifetime)app.Services.GetRequiredService<IHostApplicationLifetime>()).NotifyStarted();

		app.Run();
	}
}