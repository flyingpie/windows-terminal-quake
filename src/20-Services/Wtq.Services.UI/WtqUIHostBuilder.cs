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

		// Note that this handler needs to be called pretty early, otherwise other handles may be run first, like the AspNetCore one (if the API is enabled).
		// Note that we shouldn't ignore the return value, as it can get optimized out in "Release" mode, causing the entire handler to not work (as it gets finalized immediately).
		using var reg = PosixSignalRegistration.Create(
			PosixSignal.SIGINT,
			ctx =>
			{
				ctx.Cancel = true;

				lifetime.StopApplication(); // "Stopping"
			});

		invoker.Action = a => app?.MainWindow?.Invoke(a);

		var ui = new WtqUIHost(
			app.Services.GetRequiredService<IOptions<WtqOptions>>(),
			app.Services.GetRequiredService<IWtqBus>(),
			app.Services.GetRequiredService<IWtqWindowService>(),
			app);

		_ = new WtqHost(
			lifetime,
			app.Services.GetRequiredService<IEnumerable<IHostedService>>(),
			() => ui.Exit());

		lifetime.NotifyStarted();

		app.Run();
	}
}