using Microsoft.Extensions.Hosting.Internal;
using Microsoft.VisualStudio.Threading;
using IAsyncDisposable = System.IAsyncDisposable;

namespace Wtq.Services;

/// <summary>
/// Handles lifetime events, like "Start" and "Stop".
/// </summary>
public class WtqHost
{
	private static readonly TimeSpan InitTimeout = TimeSpan.FromSeconds(3);
	private static readonly TimeSpan StartTimeout = TimeSpan.FromSeconds(30);
	private static readonly TimeSpan StopTimeout = TimeSpan.FromSeconds(30);
	private static readonly TimeSpan DisposeTimeout = TimeSpan.FromSeconds(3);

	private static readonly ILogger _log = Utils.Log.For<WtqHost>();
	private readonly JoinableTaskFactory _taskFactory = new(JoinableTaskContext.CreateNoOpContext());

	private readonly IEnumerable<IHostedService> _hostedServices;
	private readonly IPlatformService _platformService;

	public WtqHost(
		IHostApplicationLifetime appLifetime,
		IEnumerable<IHostedService> hostedServices,
		IPlatformService platformService,
		Action onExit)
	{
		_ = Guard.Against.Null(appLifetime);
		_ = Guard.Against.Null(onExit);
		_hostedServices = Guard.Against.Null(hostedServices);
		_platformService = Guard.Against.Null(platformService);

		// Initializing + Starting
		_ = appLifetime.ApplicationStarted.Register(() =>
		{
			_taskFactory.Run(InitAsync);
			_taskFactory.Run(StartAsync);
		});

		// Stopping
		_ = appLifetime.ApplicationStopping.Register(() =>
		{
			_taskFactory.Run(StopAsync);

			((ApplicationLifetime)appLifetime).NotifyStopped(); // "Stopped"
		});

		// Stopped (dispose)
		_ = appLifetime.ApplicationStopped.Register(() =>
		{
			_taskFactory.Run(DisposeAsync);

			onExit();
		});
	}

	private async Task InitAsync()
	{
		if (_platformService.IsWtqRunning())
		{
			_log.LogWarning("WTQ seems to already be running. Running multiple instances can cause some really weird behavior, so I'm stopping this instance. If you don't see another instance already running, try killing all WTQ processes and start again.");
			Environment.Exit(-1);
		}

		var sw1 = Stopwatch.StartNew();

		_log.LogDebug("Initializing services");

		foreach (var srv in _hostedServices.OfType<WtqHostedService>())
		{
			try
			{
				_log.LogDebug("Initializing service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.InitAsync(Cancel.After(InitTimeout)).TimeoutAfterAsync(InitTimeout).NoCtx();
				_log.LogDebug("Initialized service '{Service}', took {Elapsed}", srv, sw2.Elapsed);
			}
			catch (Exception ex)
			{
				_log.LogCritical(ex, "Initializing service '{Service}' failed", srv);
				Environment.Exit(-1);
				throw;
			}
		}

		_log.LogDebug("Initialized services, took {Elapsed}", sw1.Elapsed);
	}

	private async Task StartAsync()
	{
		var sw1 = Stopwatch.StartNew();

		_log.LogDebug("Starting services");

		foreach (var srv in _hostedServices)
		{
			try
			{
				_log.LogDebug("Starting service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.StartAsync(Cancel.After(StartTimeout)).TimeoutAfterAsync(StartTimeout).NoCtx();
				_log.LogDebug("Started service '{Service}', took {Elapsed}", srv, sw2.Elapsed);
			}
			catch (Exception ex)
			{
				_log.LogCritical(ex, "Starting service '{Service}' failed", srv);
				Environment.Exit(-1);
				throw;
			}
		}

		_log.LogDebug("Started services, took {Elapsed}", sw1.Elapsed);
	}

	private async Task StopAsync()
	{
		var sw1 = Stopwatch.StartNew();

		_log.LogDebug("Stopping services");

		foreach (var srv in _hostedServices)
		{
			try
			{
				_log.LogDebug("Stopping service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.StopAsync(Cancel.After(StopTimeout)).TimeoutAfterAsync(StopTimeout).NoCtx();
				_log.LogDebug("Stopped service '{Service}', took {Elapsed}", srv, sw2.Elapsed);
			}
			catch (Exception ex)
			{
				_log.LogWarning(ex, "Stopping service '{Service}' failed", srv);
				Environment.Exit(-1);
			}
		}

		_log.LogDebug("Stopped services, took {Elapsed}", sw1.Elapsed);
	}

	private async Task DisposeAsync()
	{
		var sw1 = Stopwatch.StartNew();

		_log.LogDebug("Disposing services");

		foreach (var srv in _hostedServices.OfType<IAsyncDisposable>())
		{
			try
			{
				_log.LogDebug("Disposing service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.DisposeAsync().TimeoutAfterAsync(DisposeTimeout).NoCtx();
				_log.LogDebug("Disposed service '{Service}', took {Elapsed}", srv, sw2.Elapsed);
			}
			catch (Exception ex)
			{
				_log.LogWarning(ex, "Disposing service '{Service}' failed", srv);
				Environment.Exit(-1);
			}
		}

		_log.LogDebug("Disposed services, took {Elapsed}", sw1.Elapsed);
	}
}