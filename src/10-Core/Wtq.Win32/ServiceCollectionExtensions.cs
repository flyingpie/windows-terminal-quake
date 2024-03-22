using Microsoft.Extensions.DependencyInjection;
using Wtq.Core.Services;

namespace Wtq.Win32;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddWin32ProcessService(this IServiceCollection services)
	{
		return services
			.AddSingleton<IWtqProcessService, Win32ProcessService>();
	}
}