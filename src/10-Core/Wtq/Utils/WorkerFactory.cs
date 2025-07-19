namespace Wtq.Utils;

public sealed class WorkerFactory
{
	private readonly ILogger _log = Log.For<WorkerFactory>();

	private readonly List<Worker> _workers = [];

	public WorkerFactory(IHostApplicationLifetime lifetime)
	{
		lifetime.ApplicationStopping.Register(() =>
		{
			_log.LogInformation("Stopping workers");

			_ = Task.Run(async () =>
			{
				foreach (var w in _workers)
				{
					await w.DisposeAsync().NoCtx();
				}
			});
		});
	}

	public Worker Create(string name, TimeSpan interval, Func<CancellationToken, Task> action)
	{
		var worker = new Worker(name, interval, action);

		_workers.Add(worker);

		return worker;
	}
}