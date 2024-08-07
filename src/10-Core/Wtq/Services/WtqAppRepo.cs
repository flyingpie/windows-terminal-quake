using Microsoft.Extensions.Hosting;

namespace Wtq.Services;

public sealed class WtqAppRepo : IHostedService, IWtqAppRepo
{
	private readonly ILogger _log = Log.For<WtqAppRepo>();
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqProcessFactory _procFactory;
	private readonly IWtqProcessService _procService;
	private readonly IWtqScreenInfoProvider _screenInfoProvider;
	private readonly IWtqAppToggleService _toggleService;

	private readonly List<WtqApp> _apps = [];

	public WtqAppRepo(
		IOptionsMonitor<WtqOptions> opts,
		IWtqProcessFactory procFactory,
		IWtqProcessService procService,
		IWtqScreenInfoProvider screenInfoProvider,
		IWtqAppToggleService toggleService)
	{
		_opts = Guard.Against.Null(opts);
		_procFactory = Guard.Against.Null(procFactory);
		_procService = Guard.Against.Null(procService);
		_screenInfoProvider = Guard.Against.Null(screenInfoProvider);
		_toggleService = Guard.Against.Null(toggleService);

		// Whenever the settings file changes, update the list of tracked apps.
		opts.OnChange(o => _ = Task.Run(async () => await UpdateAppsAsync().NoCtx()));
	}

	public IEnumerable<WtqApp> Apps => _apps;

	public async Task UpdateAppsAsync()
	{
		_log.LogInformation("Updating apps");

		// Add app handles for options that don't have one yet.
		foreach (var opt in _opts.CurrentValue.Apps)
		{
			var app = GetAppByName(opt.Name);

			if (app == null)
			{
				_log.LogInformation("Missing app handle for {Options}, creating one now", opt);

				// Create & update app handle.
				app = Create(opt);
				await app.UpdateProcessAsync().NoCtx();

				_apps.Add(app);
			}
		}

		// Remove app handles for dropped options.
		foreach (var app in _apps.ToList())
		{
			var opt = GetOptionsByName(app.Name);

			if (opt == null)
			{
				_log.LogInformation("Dropped options {Options}, removing app handle {App}", opt, app);
				await app.DisposeAsync().ConfigureAwait(false);

				_apps.Remove(app);
			}
		}
	}

	public WtqApp? GetAppByName(string name)
	{
		Guard.Against.NullOrWhiteSpace(name);

		return _apps.Find(a => a.Name == name);
	}

	public WtqApp GetAppByNameRequired(string name)
	{
		return GetAppByName(name)
			?? throw new WtqException($"No instance found of type '{nameof(WtqApp)}' found with name '{name}'.");
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
		Guard.Against.Null(app);

		return new WtqApp(
			_opts,
			_procFactory,
			_procService,
			_screenInfoProvider,
			_toggleService,
			() => GetOptionsByNameRequired(app.Name),
			app.Name);
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await UpdateAppsAsync().ConfigureAwait(false);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
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
}