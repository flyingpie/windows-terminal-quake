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
		await _loop.DisposeAsync().NoCtx();

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
	public async Task<ICollection<WtqApp>> GetOnScreenAsync()
	{
		var res = new List<WtqApp>();

		foreach (var app in _apps)
		{
			if (await app.IsOnScreenAsync())
			{
				res.Add(app);
			}
		}

		return res;
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
			_toggleService,
			_screenInfoProvider,
			_windowResolver,
			() => _opts.CurrentValue.GetAppOptionsByNameRequired(app.Name),
			app.Name);
	}

	private async Task UpdateAppsAsync(bool allowStartNew)
	{
		_log.LogDebug("Updating apps");

		// Add app handles for options that don't have one yet.
		foreach (var opt in _opts.CurrentValue.Apps)
		{
			// Only update apps when their configuration is valid.
			// Otherwise, we could be missing settings that are required later on.
			if (!opt.IsValid)
			{
				_log.LogWarning("App '{App}' has validation errors, skipping during state updates", opt);
				continue;
			}

			// Fetch WtqApp object by name as defined in settings.
			var app = GetByName(opt.Name);
			if (app == null)
			{
				// Create one now if we don't have one yet.
				_log.LogInformation("Missing app handle for {Options}, creating one now", opt);

				// Create & update app handle.
				app = Create(opt);

				_apps.Add(app);
			}

			// Update the app's local state, which may include starting a new process.
			await app.UpdateLocalAppStateAsync(allowStartNew).NoCtx();
		}

		// Remove app handles for dropped options.
		foreach (var app in _apps.ToList())
		{
			var opt = _opts.CurrentValue.GetAppOptionsByName(app.Name);
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