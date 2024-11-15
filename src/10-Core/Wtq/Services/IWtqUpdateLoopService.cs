// using Microsoft.Extensions.Hosting;
//
// namespace Wtq.Services;
//
// public interface IWtqUpdateLoopService
// {
// 	void AddLoop(string name, Func<Task> action);
//
// 	void AddLoop(string name, Func<Task> action, TimeSpan interval);
// }
//
// public sealed class WtqUpdateLoopService : IWtqUpdateLoopService
// {
// 	private readonly IHostApplicationLifetime _lifetime;
// 	private readonly ILogger _log = Log.For<WtqUpdateLoopService>();
//
// 	private bool _isRunning;
//
// 	public WtqUpdateLoopService(IHostApplicationLifetime lifetime)
// 	{
// 		_lifetime = Guard.Against.Null(lifetime);
//
// 		_lifetime.ApplicationStopping.Register(() =>
// 		{
// 			_log.LogInformation("Stopping loops");
// 			_isRunning = false;
// 		});
// 	}
//
// 	public void AddLoop(string name, Func<Task> action)
// 		=> AddLoop(name, action, TimeSpan.FromMilliseconds(250));
//
// 	public void AddLoop(string name, Func<Task> action, TimeSpan interval)
// 	{
// 		Guard.Against.NullOrWhiteSpace(name);
// 		Guard.Against.Null(action);
// 		Guard.Against.OutOfRange(interval, nameof(interval), TimeSpan.FromMilliseconds(50), TimeSpan.FromSeconds(60));
//
// 		_log.LogInformation("Adding loop '{Name}'", name);
//
// 		_ = Task.Run(async () =>
// 		{
// 			while (_isRunning)
// 			{
// 				try
// 				{
// 					await action().NoCtx();
// 				}
// 				catch (Exception ex)
// 				{
// 					_log.LogWarning(ex, "Error running iteration for loop '{Name}': {Message}", name, ex.Message);
// 				}
//
// 				await Task.Delay(interval).NoCtx();
// 			}
// 		});
// 	}
// }