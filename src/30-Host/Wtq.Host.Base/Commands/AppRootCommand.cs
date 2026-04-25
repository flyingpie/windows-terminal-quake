using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System.Runtime.InteropServices;
using Wtq.Services;

namespace Wtq.Host.Base.Commands;

[Command]
public class AppRootCommand(IHostApplicationLifetime lifetime, WtqHost host) : IAsyncCommand
{
	public async Task ExecuteAsync(CancellationToken ct = default)
	{
		var l = (ApplicationLifetime)lifetime;

		// Create a task completion source that we'll wait for at the end of this command.
		// Force continuations to run on a different thread (kind of), so we don't get a deadlock.
		var exit = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

		// Trigger the task completion source when the app is signaled to exit.
		l.ApplicationStopped.Register(() =>
		{
			exit.TrySetResult();
		});

		// TODO: App doesn't properly run ApplicationLifetime.Stop and .Dispose handlers when quit through CTRL+C.
		using var reg = PosixSignalRegistration.Create(
			PosixSignal.SIGINT,
			ctx =>
			{
				// Ask the host to wait with killing the process.
				ctx.Cancel = true;

				// Tell the app to gracefully shut down.
				lifetime.StopApplication();
			}
		);

		// Tell the app we're starting, so it can run setups.
		l.NotifyStarted();

		await exit.Task.NoCtx();
	}
}