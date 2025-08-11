using Microsoft.Extensions.Hosting.Internal;
using Microsoft.VisualStudio.Threading;
using System.Runtime.InteropServices;
using IAsyncDisposable = System.IAsyncDisposable;

namespace Wtq.Services;

/// <summary>
/// Handles lifetime events, like "Start" and "Stop".
/// </summary>
public class WtqHost
{
	private readonly ILogger _log = Log.For<WtqHost>();

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
		var timeout = TimeSpan.FromSeconds(3);
		var sw1 = Stopwatch.StartNew();

		_log.LogDebug("Initializing services");

		foreach (var srv in _hostedServices.OfType<WtqHostedService>())
		{
			try
			{
				_log.LogDebug("Initializing service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.InitAsync(new CancellationTokenSource(timeout).Token).TimeoutAfterAsync(timeout);
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
		var timeout = TimeSpan.FromSeconds(3);
		var sw1 = Stopwatch.StartNew();

		_log.LogDebug("Starting services");

		foreach (var srv in _hostedServices)
		{
			try
			{
				_log.LogDebug("Starting service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.StartAsync(new CancellationTokenSource(timeout).Token).TimeoutAfterAsync(timeout);
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
		var timeout = TimeSpan.FromSeconds(3);

		_log.LogDebug("Stopping services");

		foreach (var srv in _hostedServices)
		{
			try
			{
				_log.LogDebug("Stopping service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.StopAsync(new CancellationTokenSource(timeout).Token).TimeoutAfterAsync(timeout);
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
		var timeout = TimeSpan.FromSeconds(3);

		_log.LogDebug("Disposing services");

		foreach (var srv in _hostedServices.OfType<IAsyncDisposable>())
		{
			try
			{
				_log.LogDebug("Disposing service '{Service}'", srv);
				var sw2 = Stopwatch.StartNew();
				await srv.DisposeAsync().TimeoutAfterAsync(timeout);
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