using Microsoft.Extensions.DependencyInjection;
using Wtq.Services;
using Wtq.Services.UI;

namespace Wtq.Host.Base.Commands;

[Command(Parent = typeof(AppRootCommand))]
public class GuiCommand(IWtqWindowService p, WtqUIHost host) : IAsyncCommand
{
	public Task ExecuteAsync(CancellationToken ct = default)
	{
		host.Run();

		return Task.CompletedTask;
	}
}