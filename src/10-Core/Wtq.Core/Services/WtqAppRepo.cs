namespace Wtq.Core.Services;

public sealed class WtqAppRepo : IWtqAppRepo
{
	private readonly IWtqProcessFactory _procFactory;
	private readonly IWtqProcessService _procService;
	private readonly IWtqAppToggleService _toggleService;
	private readonly IOptionsMonitor<WtqOptions> _opts;

	private readonly List<WtqApp> _apps = [];

	public WtqAppRepo(
		IOptionsMonitor<WtqOptions> opts,
		IWtqProcessFactory procFactory,
		IWtqProcessService procService,
		IWtqAppToggleService toggleService)
	{
		_opts = Guard.Against.Null(opts, nameof(opts));
		_procFactory = Guard.Against.Null(procFactory, nameof(procFactory));
		_procService = Guard.Against.Null(procService, nameof(procService));
		_toggleService = Guard.Against.Null(toggleService, nameof(toggleService));
	}

	public IReadOnlyCollection<WtqApp> Apps => _apps;

	public async Task UpdateAppsAsync()
	{
		foreach (var opt in _opts.CurrentValue.Apps)
		{
			var app = GetAppByName(opt.Name);

			if (app == null)
			{
				_apps.Add(Create(opt));
			}
		}

		foreach (var app in _apps.ToList())
		{
			var opt = GetOptionsByName(app.Name);

			if (opt == null)
			{
				await app.DisposeAsync().ConfigureAwait(false);

				_apps.Remove(app);
			}
		}
	}

	public WtqApp? GetAppByName(string name)
	{
		Guard.Against.NullOrWhiteSpace(name, nameof(name));

		return _apps.Find(a => a.Name == name);
	}

	public WtqApp GetAppByNameRequired(string name)
	{
		return GetAppByName(name)
			?? throw new WtqException($"No instance found of type '{nameof(WtqApp)}' found with name '{name}'.");
	}

	public WtqAppOptions? GetOptionsByName(string name)
	{
		Guard.Against.NullOrWhiteSpace(name, nameof(name));

		return _opts.CurrentValue.Apps.FirstOrDefault(o => o.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false);
	}

	public WtqAppOptions GetOptionsByNameRequired(string name)
	{
		return GetOptionsByName(name)
			?? throw new WtqException($"No instance found of type '{nameof(WtqAppOptions)}' found with name '{name}'.");
	}

	public async ValueTask DisposeAsync()
	{
		foreach (var app in Apps)
		{
			await app.DisposeAsync().ConfigureAwait(false);
		}
	}

	public WtqApp Create(WtqAppOptions app)
	{
		Guard.Against.Null(app, nameof(app));

		return new WtqApp(
			_procFactory,
			_procService,
			_toggleService,
			() => GetOptionsByNameRequired(app.Name),
			app.Name);
	}
}