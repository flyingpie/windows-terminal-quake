using Wtq.Core.Configuration;
using Wtq.Core.Services;

namespace Wtq.Services;

public interface IWtqProcessFactory
{
	WtqApp Create(WtqAppOptions app);
}

public class WtqProcessFactory(
	IWtqAppToggleService toggleService,
	IWtqProcessService procService)
	: IWtqProcessFactory
{
	private readonly IWtqAppToggleService _toggleService = toggleService ?? throw new ArgumentNullException(nameof(toggleService));
	private readonly IWtqProcessService _procService = procService ?? throw new ArgumentNullException(nameof(procService));

	public WtqApp Create(WtqAppOptions app)
	{
		if (app == null)
		{
			throw new ArgumentNullException(nameof(app));
		}

		return new WtqApp(_procService, _toggleService)
		{
			App = app,
		};
	}
}