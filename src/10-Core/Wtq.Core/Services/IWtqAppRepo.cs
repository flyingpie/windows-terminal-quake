using Microsoft.Extensions.Options;
using Wtq.Configuration;
using Wtq.Services;

namespace Wtq.Core.Services;

public interface IWtqAppRepo
{
	IReadOnlyCollection<WtqApp> Apps { get; }

	WtqApp? GetProcessForApp(WtqAppOptions app);
}

public class WtqAppRepo : IWtqAppRepo
{
	private readonly IWtqProcessFactory _appFactory;

	private readonly List<WtqApp> _apps = [];

	public IReadOnlyCollection<WtqApp> Apps => _apps;

	public WtqAppRepo(
		IOptionsMonitor<WtqOptions> opts,
		IWtqProcessFactory appFactory)
	{
		_appFactory = appFactory;

		_apps = opts.CurrentValue.Apps
			.Select(app => appFactory.Create(app))
			.ToList();
	}

	public WtqApp? GetProcessForApp(WtqAppOptions app)
	{
		return _apps.FirstOrDefault(a => a.App.Name == app.Name);
	}
}