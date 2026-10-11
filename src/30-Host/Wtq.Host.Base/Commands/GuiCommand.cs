using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Wtq.Services;
using Wtq.Services.UI;

namespace Wtq.Host.Base.Commands;

[Command(Parent = typeof(AppRootCommand))]
public class GuiCommand(
	IHostApplicationLifetime lifetime,
	IWtqWindowService p,
	WtqUIHost host,
	WtqHost h
) : IAsyncCommand
{
	public Task ExecuteAsync(CancellationToken ct = default)
	{
		var l = (ApplicationLifetime)lifetime;

		// Tell the app we're starting, so it can run setups.
		l.NotifyStarted();

		host.Run();

		// Tell the app to gracefully shut down.
		lifetime.StopApplication();

		return Task.CompletedTask;
	}
}
