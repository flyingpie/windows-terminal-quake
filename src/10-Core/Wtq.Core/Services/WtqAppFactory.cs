using Wtq.Core.Configuration;
using Wtq.Core.Services;

namespace Wtq.Services;

public class WtqAppFactory(
	IWtqAppToggleService toggleService,
	IWtqBus bus,
	IWtqProcessFactory _procFactory,
	IWtqProcessService procService)
	: IWtqAppFactory
{
	private readonly IWtqAppToggleService _toggleService = toggleService ?? throw new ArgumentNullException(nameof(toggleService));
	private readonly IWtqBus _bus = bus;
	private readonly IWtqProcessFactory procFactory = _procFactory ?? throw new ArgumentNullException(nameof(procFactory));
	private readonly IWtqProcessService _procService = procService ?? throw new ArgumentNullException(nameof(procService));

	public WtqApp Create(WtqAppOptions app)
	{
		ArgumentNullException.ThrowIfNull(app);

		return new WtqApp(
			_procFactory,
			_procService,
			_toggleService,
			_bus,
			app);
	}
}