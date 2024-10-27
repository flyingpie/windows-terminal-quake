namespace Wtq.Services;

public sealed class WtqAppRepo : IWtqAppRepo
{
	private readonly ILogger _log = Log.For<WtqAppRepo>();
	private readonly IWtqBus _bus;
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqWindowResolver _windowResolver;
	private readonly IWtqScreenInfoProvider _screenInfoProvider;
	private readonly IWtqAppToggleService _toggleService;

	private readonly List<WtqApp> _apps = [];

	public WtqAppRepo(
		IOptionsMonitor<WtqOptions> opts,
		IWtqBus bus,
		IWtqWindowResolver procResolver,
		IWtqScreenInfoProvider screenInfoProvider,
		IWtqAppToggleService toggleService)
	{
		_opts = Guard.Against.Null(opts);
		_bus = Guard.Against.Null(bus);
		_windowResolver = Guard.Against.Null(procResolver);
		_screenInfoProvider = Guard.Against.Null(screenInfoProvider);
		_toggleService = Guard.Against.Null(toggleService);

		// Whenever the settings file changes, update the list of tracked apps.
		opts.OnChange(o => _ = Task.Run(async () => await UpdateAppsAsync(allowStartNew: false).NoCtx()));
	}

	public async Task InitializeAsync()
	{
		// TODO: Make setting for "allowStartNew"? As in, allow starting apps on WTQ first start?
		// "StartApps": "OnWtqStart | OnHotkeyPress"
		await UpdateAppsAsync(allowStartNew: true).NoCtx();
	}

	public async ValueTask DisposeAsync()
	{
		foreach (var app in _apps)
		{
			await app.DisposeAsync().NoCtx();
		}
	}

	public WtqApp? GetByName(string name)
	{
		Guard.Against.NullOrWhiteSpace(name);

		return _apps.Find(a => a.Name == name);
	}

	public WtqApp? GetByWindow(WtqWindow window)
	{
		Guard.Against.Null(window);

		return _apps.FirstOrDefault(a => a.Window == window);
	}

	public WtqApp? GetPrimary()
	{
		return _apps.FirstOrDefault();
	}

	private WtqAppOptions? GetOptionsByName(string name)
	{
		Guard.Against.NullOrWhiteSpace(name);

		return _opts.CurrentValue.Apps.FirstOrDefault(o => o.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false);
	}

	private WtqAppOptions GetOptionsByNameRequired(string name)
	{
		return GetOptionsByName(name)
			?? throw new WtqException($"No instance found of type '{nameof(WtqAppOptions)}' found with name '{name}'.");
	}

	private WtqApp Create(WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return new WtqApp(
			_opts,
			this,
			_bus,
			_windowResolver,
			_screenInfoProvider,
			_toggleService,
			() => GetOptionsByNameRequired(app.Name),
			app.Name);
	}

	private async Task UpdateAppsAsync(bool allowStartNew)
	{
		_log.LogInformation("Updating apps");

		// Add app handles for options that don't have one yet.
		foreach (var opt in _opts.CurrentValue.Apps)
		{
			var app = GetByName(opt.Name);

			if (app != null)
			{
				continue;
			}

			_log.LogInformation("Missing app handle for {Options}, creating one now", opt);

			// Create & update app handle.
			app = Create(opt);

			await app.UpdateLocalAppStateAsync(allowStartNew).NoCtx();

			_apps.Add(app);
		}

		// Remove app handles for dropped options.
		foreach (var app in _apps.ToList())
		{
			var opt = GetOptionsByName(app.Name);

			if (opt != null)
			{
				continue;
			}

			_log.LogInformation("Dropped options {Options}, removing app handle {App}", opt, app);
			await app.DisposeAsync().NoCtx();

			_apps.Remove(app);
		}
	}
}