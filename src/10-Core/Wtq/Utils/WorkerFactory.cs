// namespace Wtq.Utils;
//
// public sealed class WorkerFactory : WtqHostedService
// {
// 	private readonly ILogger _log = Log.For<WorkerFactory>();
//
// 	private readonly List<Worker> _workers = [];
//
// 	public WorkerFactory(IHostApplicationLifetime lifetime)
// 	{
// 		lifetime.ApplicationStopping.Register(() => { });
// 	}
//
// 	public void Create(string name, TimeSpan interval, Func<CancellationToken, Task> action)
// 	{
// 		_log.LogInformation("Creating worker with name '{Name}', and interval '{Interval}'", name, interval);
//
// 		var worker = new Worker(name, interval, action);
//
// 		_workers.Add(worker);
//
// 		// return worker;
// 	}
//
// 	protected override async Task OnStopAsync(CancellationToken cancellationToken)
// 	{
// 		_log.LogInformation("Stopping workers");
// 		Console.WriteLine("##### Stopping workers");
//
// 		foreach (var w in _workers)
// 		{
// 			await w.DisposeAsync().NoCtx();
// 		}
//
// 		_log.LogInformation("Stopped workers");
// 		Console.WriteLine("##### Stopped workers");
// 	}
// }