namespace Wtq.Services;

/// <inheritdoc cref="IWtqAppRepo"/>.
public sealed class WtqAppRepo : IWtqAppRepo
{
	private readonly ILogger _log = Log.For<WtqAppRepo>();
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqAppToggleService _toggleService;
	private readonly IWtqScreenInfoProvider _screenInfoProvider;
	private readonly IWtqWindowResolver _windowResolver;

	private readonly List<WtqApp> _apps = [];
	private readonly Worker _loop;

	public WtqAppRepo(
		IHostApplicationLifetime lifetime,
		IOptionsMonitor<WtqOptions> opts,
		IWtqAppToggleService toggleService,
		IWtqScreenInfoProvider screenInfoProvider,
		IWtqWindowResolver procResolver)
	{
		_ = Guard.Against.Null(lifetime);
		_opts = Guard.Against.Null(opts);
		_toggleService = Guard.Against.Null(toggleService);
		_screenInfoProvider = Guard.Against.Null(screenInfoProvider);
		_windowResolver = Guard.Against.Null(procResolver);

		// Whenever the settings change, update the list of tracked apps.
		opts.OnChange(o => _ = Task.Run(() => UpdateAppsAsync(allowStartNew: false)));

		_ = Task.Run(async () =>
		{
			// TODO: Make setting for "allowStartNew"? As in, allow starting apps on WTQ first start?
			// "StartApps": "OnWtqStart | OnHotkeyPress"
			await UpdateAppsAsync(allowStartNew: true).NoCtx();
		});

		// When WTQ stops, reset all tracked apps.
		lifetime.ApplicationStopping.Register(() =>
		{
			// TODO: Find a nicer way to handle this.
			DisposeAsync().GetAwaiter().GetResult();
		});

		// Start loop that updates app state periodically.
		_loop = new(
			$"{nameof(WtqAppRepo)}.UpdateAppStates",
			_ => UpdateAppsAsync(allowStartNew: false),
			TimeSpan.FromSeconds(1));
	}

	public async ValueTask DisposeAsync()
	{
		foreach (var app in _apps)
		{
			await app.DisposeAsync().NoCtx();
		}
	}

	/// <inheritdoc/>
	public IEnumerable<WtqApp> GetAll()
	{
		return _apps;
	}

	/// <inheritdoc/>
	public WtqApp? GetByName(string name)
	{
		Guard.Against.NullOrWhiteSpace(name);

		return _apps.Find(a => a.Name == name);
	}

	/// <inheritdoc/>
	public WtqApp? GetByWindow(WtqWindow window)
	{
		Guard.Against.Null(window);

		return _apps.FirstOrDefault(a => a.Window == window);
	}

	/// <inheritdoc/>
	public WtqApp? GetOpen()
	{
		return _apps.FirstOrDefault(a => a.IsOpen);
	}

	/// <inheritdoc/>
	public WtqApp? GetPrimary()
	{
		return _apps.FirstOrDefault();
	}

	private WtqApp Create(WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return new WtqApp(
			_opts,
			_toggleService,
			_screenInfoProvider,
			_windowResolver,
			() => GetOptionsByNameRequired(app.Name),
			app.Name);
	}

	private WtqAppOptions? GetOptionsByName(string name)
	{
		Guard.Against.NullOrWhiteSpace(name);

		return _opts
			.CurrentValue
			.Apps
			.FirstOrDefault(o => o.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false);
	}

	private WtqAppOptions GetOptionsByNameRequired(string name)
	{
		return GetOptionsByName(name)
			?? throw new WtqException($"No instance found of type '{nameof(WtqAppOptions)}' found with name '{name}'. These were found: '{string.Join(", ", _opts.CurrentValue.Apps.Select(a => a.Name))}'.");
	}

	private async Task UpdateAppsAsync(bool allowStartNew)
	{
		_log.LogDebug("Updating apps");

		// Add app handles for options that don't have one yet.
		foreach (var opt in _opts.CurrentValue.Apps)
		{
			if (!opt.IsValid)
			{
				_log.LogWarning("App '{App}' has validation errors, skipping during state updates", opt);
				continue;
			}

			var app = GetByName(opt.Name);

			if (app == null)
			{
				_log.LogInformation("Missing app handle for {Options}, creating one now", opt);

				// Create & update app handle.
				app = Create(opt);
				// continue;
				_apps.Add(app);
			}

			await app.UpdateLocalAppStateAsync(allowStartNew).NoCtx();
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