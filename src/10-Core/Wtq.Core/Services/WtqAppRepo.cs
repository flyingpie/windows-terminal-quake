using Wtq.Configuration;
using Wtq.Services;

namespace Wtq.Core.Services;

public sealed class WtqAppRepo : IWtqAppRepo
{
	private readonly List<WtqApp> _apps;

	public WtqAppRepo(
		IOptionsMonitor<WtqOptions> opts,
		IWtqProcessFactory appFactory)
	{
		Guard.Against.Null(opts);
		Guard.Against.Null(appFactory);

		_apps = opts.CurrentValue.Apps
			.Select(appFactory.Create)
			.ToList();
	}

	public IReadOnlyCollection<WtqApp> Apps => _apps;

	public WtqApp? GetProcessForApp(WtqAppOptions app)
	{
		return _apps.Find(a => a.App.Name == app.Name);
	}

	public void Dispose()
	{
		foreach (var app in Apps)
		{
			app.Dispose();
		}
	}
}