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

	private static readonly ILogger Log = Utils.Log.For<WtqHost>();

	private readonly IEnumerable<IHostedService> _hostedServices;
	private readonly JoinableTaskFactory _taskFactory = new(JoinableTaskContext.CreateNoOpContext());

	public WtqHost(
		IHostApplicationLifetime appLifetime,
		IEnumerable<IHostedService> hostedServices,
		Action onExit)
	{
		_ = Guard.Against.Null(appLifetime);
		_ = Guard.Against.Null(onExit);
		_hostedServices = Guard.Against.Null(hostedServices);

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
		var sw1 = Stopwatch.StartNew();

		Log.LogDebug("Initializing services");

		foreach (var srv in _hostedServices.OfType<WtqHostedService>())
		{
			try
			{
				Log.LogDebug("Initializing service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.InitAsync(Cancel.After(InitTimeout)).TimeoutAfterAsync(InitTimeout).NoCtx();
				Log.LogDebug("Initialized service '{Service}', took {Elapsed}", srv, sw2.Elapsed);
			}
			catch (Exception ex)
			{
				Log.LogCritical(ex, "Initializing service '{Service}' failed", srv);
				Environment.Exit(-1);
				throw;
			}
		}

		Log.LogDebug("Initialized services, took {Elapsed}", sw1.Elapsed);
	}

	private async Task StartAsync()
	{
		var sw1 = Stopwatch.StartNew();

		Log.LogDebug("Starting services");

		foreach (var srv in _hostedServices)
		{
			try
			{
				Log.LogDebug("Starting service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.StartAsync(Cancel.After(StartTimeout)).TimeoutAfterAsync(StartTimeout).NoCtx();
				Log.LogDebug("Started service '{Service}', took {Elapsed}", srv, sw2.Elapsed);
			}
			catch (Exception ex)
			{
				Log.LogCritical(ex, "Starting service '{Service}' failed", srv);
				Environment.Exit(-1);
				throw;
			}
		}

		Log.LogDebug("Started services, took {Elapsed}", sw1.Elapsed);
	}

	private async Task StopAsync()
	{
		var sw1 = Stopwatch.StartNew();

		Log.LogDebug("Stopping services");

		foreach (var srv in _hostedServices)
		{
			try
			{
				Log.LogDebug("Stopping service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.StopAsync(Cancel.After(StopTimeout)).TimeoutAfterAsync(StopTimeout).NoCtx();
				Log.LogDebug("Stopped service '{Service}', took {Elapsed}", srv, sw2.Elapsed);
			}
			catch (Exception ex)
			{
				Log.LogWarning(ex, "Stopping service '{Service}' failed", srv);
				Environment.Exit(-1);
			}
		}

		Log.LogDebug("Stopped services, took {Elapsed}", sw1.Elapsed);
	}

	private async Task DisposeAsync()
	{
		var sw1 = Stopwatch.StartNew();

		Log.LogDebug("Disposing services");

		foreach (var srv in _hostedServices.OfType<IAsyncDisposable>())
		{
			try
			{
				Log.LogDebug("Disposing service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.DisposeAsync().TimeoutAfterAsync(DisposeTimeout).NoCtx();
				Log.LogDebug("Disposed service '{Service}', took {Elapsed}", srv, sw2.Elapsed);
			}
			catch (Exception ex)
			{
				Log.LogWarning(ex, "Disposing service '{Service}' failed", srv);
				Environment.Exit(-1);
			}
		}

		Log.LogDebug("Disposed services, took {Elapsed}", sw1.Elapsed);
	}
}