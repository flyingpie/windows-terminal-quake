namespace Wtq.Services;

/// <inheritdoc cref="IWtqAppRepo"/>.
public sealed class WtqAppRepo : WtqHostedService, IWtqAppRepo
{
	private readonly ILogger _log = Log.For<WtqAppRepo>();
	private readonly WtqSemaphoreSlim _updateLock = new(1, 1);

	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqAppToggleService _toggleService;
	private readonly IWtqScreenInfoProvider _screenInfoProvider;
	private readonly IWtqWindowResolver _windowResolver;

	private readonly ConcurrentDictionary<string, WtqApp> _apps = new(StringComparer.OrdinalIgnoreCase);
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
		_screenInfoProvider = Guard.Against.Null(screenInfoProvider);
		_toggleService = Guard.Against.Null(toggleService);
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
			_ = Task.Run(DisposeAsync);
		});

		// Start loop that updates app state periodically.
		_loop = new(
			$"{nameof(WtqAppRepo)}.UpdateAppStates",
			TimeSpan.FromSeconds(1),
			_ => UpdateAppsAsync(allowStartNew: false));
	}

	protected override async Task OnStopAsync(CancellationToken cancellationToken)
	{
		await _loop.DisposeAsync().NoCtx();

		foreach (var app in _apps)
		{
			await app.Value.DisposeAsync().NoCtx();
		}
	}

	/// <inheritdoc/>
	public IEnumerable<WtqApp> GetAll()
	{
		return _apps.Values;
	}

	/// <inheritdoc/>
	public WtqApp? GetByName(string name)
	{
		Guard.Against.NullOrWhiteSpace(name);

		return _apps.Values.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
	}

	/// <inheritdoc/>
	public WtqApp? GetByWindow(WtqWindow window)
	{
		Guard.Against.Null(window);

		return _apps.Values.FirstOrDefault(a => a.Window == window);
	}

	/// <inheritdoc/>
	public WtqApp? GetOpen()
	{
		return _apps.Values.FirstOrDefault(a => a.IsOpen);
	}

	/// <inheritdoc/>
	public WtqApp? GetPrimary()
	{
		return _apps.Values.FirstOrDefault();
	}

	/// <summary>
	/// Returns the <see cref="WtqApp"/> that is associated with the specified <paramref name="opts"/>, creating one if one doesn't exist yet.
	/// </summary>
	private WtqApp GetOrCreate(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return _apps.GetOrAdd(
			opts.Name,
			appName =>
			{
				_log.LogDebug("Creating app handle for '{AppName}'", appName);

				return new WtqApp(
					_toggleService,
					_screenInfoProvider,
					_windowResolver,
					() => _opts.CurrentValue.GetAppOptionsByNameRequired(appName),
					appName);
			});
	}

	/// <summary>
	/// Updates the list of tracked apps (<see cref="WtqApp"/>), to match the list of available option objects (<see cref="WtqAppOptions"/>).
	/// </summary>
	private async Task UpdateAppsAsync(bool allowStartNew)
	{
		// Make sure this method always runs non-concurrently.
		using var l = await _updateLock.WaitAsync(new CancellationTokenSource(TimeSpan.FromSeconds(15)).Token);

		_log.LogDebug("Updating apps (allow start new: {AllowStartNow})", allowStartNew);

		// Add app handles for options that don't have one yet.
		foreach (var opt in _opts.CurrentValue.Apps)
		{
			// Fetch WtqApp object by name as defined in settings.
			var app = GetOrCreate(opt);

			// Update the app's local state, which may include starting a new process.
			await app.UpdateLocalAppStateAsync(allowStartNew).NoCtx();
		}

		// Remove app handles for dropped options.
		foreach (var app in _apps.ToList())
		{
			var opt = _opts.CurrentValue.GetAppOptionsByName(app.Value.Name);
			if (opt != null)
			{
				continue;
			}

			_log.LogInformation("Dropped options {Options}, removing app handle {App}", opt, app);

			await app.Value.DisposeAsync().NoCtx();

			if (!_apps.TryRemove(app.Key, out _))
			{
				_log.LogWarning("Could not remove app with name '{AppName}' from repo, maybe it was already removed and this is some race-condition sorta thing?", app.Key);
			}
		}
	}
}