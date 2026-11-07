using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Wtq.Services;
using Wtq.Services.UI;

namespace Wtq.Host.Base;

public abstract class WtqHostBase
{
	public async Task RunAsync(string[] args)
	{
		Guard.Against.Null(args);

		var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);

		var platform = CreatePlatformService();

		// Setup logging ASAP, so we can log stuff if initialization goes awry.
		Log.Configure(platform.PathToLogsDir);

		AppDomain.CurrentDomain.ProcessExit += (s, a) =>
		{
			var log = Log.For<WtqHostBase>();
			log.LogInformation("Process exit");

			// Make sure all logs are written to disk (i.e., otherwise soem logs may still be buffered without being flushed before the app exits).
			Log.CloseAndFlush();
		};
		// csharpier-ignore
		builder
			.Services
			.AddConfiguration(platform)
			.AddLogging()
			.AddSingleton(platform)
			.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>()
			.AddUI()
			.AddWtqCore();

		ConfigureServices(builder.Services);

		var host = builder.Build();
		await host.RunAsync();
	}

	protected abstract IPlatformService CreatePlatformService();

	protected virtual void ConfigureServices(IServiceCollection services)
	{
		// Implemented by OS-specific implementations.
	}
}
