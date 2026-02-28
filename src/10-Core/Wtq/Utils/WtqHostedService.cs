namespace Wtq.Utils;

/// <summary>
/// Wrapper around <see cref="IHostedService"/> and <see cref="IAsyncDisposable"/>, makes inheriting
/// types more to the point, by only overriding which of these are necessary.
/// <ul>
/// <li>StartAsync</li>
/// <li>StopAsync</li>
/// <li>DisposeAsync</li>
/// </ul>
/// </summary>
public abstract class WtqHostedService
	: IAsyncDisposable, IHostedService
{
	private readonly ILogger _log;
	private readonly string _name;

	protected WtqHostedService()
	{
		_log = Log.For(GetType());
		_name = GetType().FullName!;
	}

	public async Task InitAsync(CancellationToken cancellationToken)
	{
		_log.LogDebug("Initializing service '{Name}'", _name);

		var sw = Stopwatch.StartNew();

		try
		{
			await OnInitAsync(cancellationToken).NoCtx();

			_log.LogDebug("Initialized service '{Name}' (took {Elapsed})", _name, sw.Elapsed);
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error initializing hosted service '{HostedService}': {Message}", GetType().FullName, ex.Message);
			throw;
		}
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		_log.LogDebug("Starting service '{Name}'", _name);

		var sw = Stopwatch.StartNew();

		try
		{
			await OnStartAsync(cancellationToken).NoCtx();

			_log.LogDebug("Started service '{Name}' (took {Elapsed})", _name, sw.Elapsed);
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error starting hosted service '{HostedService}': {Message}", GetType().FullName, ex.Message);
			throw;
		}
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		_log.LogDebug("Stopping service '{Name}'", _name);

		var sw = Stopwatch.StartNew();

		try
		{
			await OnStopAsync(cancellationToken).NoCtx();

			_log.LogDebug("Stopped service '{Name}' (took {Elapsed})", _name, sw.Elapsed);
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error stopping hosted service '{HostedService}': {Message}", GetType().FullName, ex.Message);
			throw;
		}
	}

	public async ValueTask DisposeAsync()
	{
		_log.LogDebug("Disposing service '{Name}'", _name);

		var sw = Stopwatch.StartNew();

		try
		{
			await OnDisposeAsync().NoCtx();

			_log.LogDebug("Disposed service '{Name}' (took {Elapsed})", _name, sw.Elapsed);
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error disposing hosted service '{HostedService}': {Message}", GetType().FullName, ex.Message);
			throw;
		}
	}

	protected virtual Task OnInitAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	protected virtual Task OnStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	protected virtual Task OnStopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	protected virtual ValueTask OnDisposeAsync() => ValueTask.CompletedTask;
}