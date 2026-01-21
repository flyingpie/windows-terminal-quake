using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Wtq.Services;
using Wtq.Services.UI;

namespace Wtq.Host.Base.Commands;

[Command]
public class AppRootCommand(IHostApplicationLifetime lifetime, WtqHost host) : IAsyncCommand
{
	public async Task ExecuteAsync(CancellationToken ct = default)
	{
		var l = (ApplicationLifetime)lifetime;

		using var reg = PosixSignalRegistration.Create(
			PosixSignal.SIGINT,
			ctx =>
			{
				ctx.Cancel = true;

				lifetime.StopApplication(); // "Stopping"
			});

		l.NotifyStarted();

		var exit = new TaskCompletionSource();

		l.ApplicationStopped.Register(() => exit.SetResult());

		await exit.Task.NoCtx();
	}

	public static void RunHeadless()
	{
		// Guard.Against.Null(services);

		// using var invoker = new WtqUIInvoker();

		// s
		// 	.AddLogging()
		// 	.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>()
		// 	// .AddSingleton<IWtqUIService>(_ => invoker)
		// ;

		// services(s);

		// var p = s.BuildServiceProvider();

		// var lifetime = (ApplicationLifetime)p.GetRequiredService<IHostApplicationLifetime>();

		// Note that this handler needs to be called pretty early, otherwise other handles may be run first, like the AspNetCore one (if the API is enabled).
		// Note that we shouldn't ignore the return value, as it can get optimized out in "Release" mode, causing the entire handler to not work (as it gets finalized immediately).
		// using var reg = PosixSignalRegistration.Create(
		// 	PosixSignal.SIGINT,
		// 	ctx =>
		// 	{
		// 		ctx.Cancel = true;
		//
		// 		lifetime.StopApplication(); // "Stopping"
		// 	});

		// invoker.Action = a => a();

		// _ = new WtqHost(
		// 	lifetime,
		// 	p.GetRequiredService<IEnumerable<IHostedService>>(),
		// 	p.GetRequiredService<IPlatformService>(),
		// 	() => {});

		// lifetime.NotifyStarted();
		//
		// var exit = new TaskCompletionSource();
		//
		// lifetime.ApplicationStopped.Register(() => exit.SetResult());
		//
		// exit.Task.GetAwaiter().GetResult();
	}
}