using Microsoft.Extensions.DependencyInjection;
using Wtq.Services;

namespace Wtq.Utils;

public class AsyncServiceInitializer
{
	public List<ServiceDescriptor> Services { get; } = [];

	public async Task InitializeAsync(IServiceProvider provider)
	{
		var serv = Services
			.Select(
				s => new
				{
					Descr = s,
					Inst = (IAsyncInitializable)provider.GetService(s.ServiceType),
				})
			.OrderBy(s => s.Inst.InitializePriority)
			.ToList();

		foreach (var s in serv)
		{
			Console.WriteLine($"Async init (pre): '{s.Inst.GetType().FullName}'");
		}

		foreach (var s in serv)
		{
			Console.WriteLine($"Async init: '{s.Inst.GetType().FullName}'");
			await s.Inst.InitializeAsync().NoCtx();
		}
	}
}