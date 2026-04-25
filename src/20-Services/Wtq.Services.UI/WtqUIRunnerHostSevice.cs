using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Wtq.Services.UI;

/// <summary>
/// Listens for <see cref="WtqUIRequestedEvent"/> events and starts the GUI as a separate process when sent.
/// </summary>
public class WtqUIRunnerHostSevice(IPlatformService platformService, IWtqBus bus) : WtqHostedService
{
	private readonly ILogger _log = Log.For<WtqUIRunnerHostSevice>();
	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private readonly IPlatformService _platformService = Guard.Against.Null(platformService);

	protected override Task OnInitAsync(CancellationToken cancellationToken)
	{
		_bus.OnEvent<WtqUIRequestedEvent>(_ =>
		{
			_log.LogInformation("Opening GUI, using command '{Command} gui'", _platformService.PathToAppExe);

			Process.Start(new ProcessStartInfo()
			{
				FileName = _platformService.PathToAppExe,
				ArgumentList = { "gui" },
			});

			return Task.CompletedTask;
		});

		return Task.CompletedTask;
	}
}