// using Microsoft.Extensions.DependencyInjection;
//
// namespace Wtq.Utils.AsyncInit;
//
// /// <summary>
// /// Container class for storing and initializing services flagged with <see cref="IAsyncInitializable"/>.
// /// </summary>
// public class AsyncServiceInitializer
// {
// 	private readonly ILogger _log = Log.For<AsyncServiceInitializer>();
//
// 	public List<ServiceDescriptor> Services { get; } = [];
//
// 	public async Task InitializeAsync(IServiceProvider provider)
// 	{
// 		Guard.Against.Null(provider);
//
// 		var serv = Services
// 			.Select(s => new
// 			{
// 				Descr = s,
// 				Inst = (IAsyncInitializable)provider.GetService(s.ServiceType)!,
// 			})
// 			.OrderByDescending(s => s.Inst.InitializePriority)
// 			.ToList();
//
// 		foreach (var s in serv)
// 		{
// 			_log.LogDebug("Calling async initializer for type '{Type}'", s.GetType().FullName);
// 			await s.Inst.InitializeAsync().NoCtx();
// 		}
// 	}
// }