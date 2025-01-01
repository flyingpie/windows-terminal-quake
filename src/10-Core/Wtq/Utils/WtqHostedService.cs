namespace Wtq.Utils;

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

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		_log.LogDebug("Starting service '{Name}'", _name);

		var sw = Stopwatch.StartNew();

		await OnStartAsync(cancellationToken).NoCtx();

		_log.LogDebug("Started service '{Name}' (took {Elapsed})", _name, sw.Elapsed);
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		_log.LogDebug("Stopping service '{Name}'", _name);

		var sw = Stopwatch.StartNew();

		await OnStopAsync(cancellationToken).NoCtx();

		_log.LogDebug("Stopped service '{Name}' (took {Elapsed})", _name, sw.Elapsed);
	}

	public async ValueTask DisposeAsync()
	{
		_log.LogDebug("Disposing service '{Name}'", _name);

		var sw = Stopwatch.StartNew();

		await OnDisposeAsync().NoCtx();

		_log.LogDebug("Disposed service '{Name}' (took {Elapsed})", _name, sw.Elapsed);
	}

	protected virtual Task OnStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	protected virtual Task OnStopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	protected virtual ValueTask OnDisposeAsync() => ValueTask.CompletedTask;
}