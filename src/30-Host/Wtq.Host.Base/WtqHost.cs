using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Wtq.Host.Base;

public class WtqHost
{
	public async Task RunAsync(Action<IServiceCollection> services, string[] args)
	{
		Guard.Against.Null(args);

		var platform = PlatformServiceFactory.Create();

		// Setup logging ASAP, so we can log stuff if initialization goes awry.
		Log.Configure(platform.PathToLogsDir);

		var builder = Microsoft.Extensions.Hosting.Host.CreateEmptyApplicationBuilder(new());

		AppDomain.CurrentDomain.ProcessExit += (s, a) =>
		{
			var log = Log.For<WtqHost>();
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
			.AddWtqCore();

		services(builder.Services);

		var host = builder.Build();

		try
		{
			await host.RunAsync();
		}
		catch (Exception ex)
		{
			Log.For<WtqHost>().LogError(ex, "Error running WTQ: {Message}", ex.Message);

			// Make sure all logs are written to disk (i.e., otherwise soem logs may still be buffered without being flushed before the app exits).
			Log.CloseAndFlush();
		}
	}
}
