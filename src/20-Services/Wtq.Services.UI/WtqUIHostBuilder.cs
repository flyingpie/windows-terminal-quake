using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using Photino.Blazor;
using System.Runtime.InteropServices;
using Wtq.Configuration;

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

		var lifetime = (ApplicationLifetime)app.Services.GetRequiredService<IHostApplicationLifetime>();

		PosixSignalRegistration.Create(
			PosixSignal.SIGINT,
			ctx =>
			{
				ctx.Cancel = true;

				lifetime.StopApplication(); // "Stopping"
			});

		invoker.Action = a => app?.MainWindow?.Invoke(a);

		new WtqUIHost(
			app.Services.GetRequiredService<IOptions<WtqOptions>>(),
			app.Services.GetRequiredService<IEnumerable<IHostedService>>(),
			lifetime,
			app.Services.GetRequiredService<IWtqBus>(),
			app.Services.GetRequiredService<IWtqWindowService>(),
			app);

		lifetime.NotifyStarted();

		app.Run();
	}
}