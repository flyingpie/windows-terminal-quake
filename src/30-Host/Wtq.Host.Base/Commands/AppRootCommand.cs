using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Wtq.Services;

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
			}
		);

		l.NotifyStarted();

		var exit = new TaskCompletionSource();

		l.ApplicationStopped.Register(() => exit.SetResult());

		await exit.Task.NoCtx();
	}
}
