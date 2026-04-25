using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Wtq.Host.Base.Commands;
using Wtq.Services;
using Wtq.Services.API;
using Wtq.Services.UI;

namespace Wtq.Host.Base;

public abstract class WtqHostBase
{
	public async Task RunAsync(string[] args)
	{
		Guard.Against.Null(args);

		var platform = CreatePlatformService();

		// Setup logging ASAP, so we can log stuff if initialization goes awry.
		Log.Configure(platform.PathToLogsDir);

		await RunCliAsync(platform, args).NoCtx();
	}

	protected abstract IPlatformService CreatePlatformService();

	protected virtual void ConfigureServices(IServiceCollection services)
	{
		// Implemented by OS-specific implementations.
	}

	/// <summary>
	/// This is called whenever the app is being run in CLI mode (i.e. with command line arguments).
	/// </summary>
	private async Task<int> RunCliAsync(IPlatformService platform, string[] args)
	{
		AppDomain.CurrentDomain.ProcessExit += (s, a) =>
		{
			var log = Log.For<WtqHostBase>();
			log.LogInformation("Process exit");

			// Make sure all logs are written to disk (i.e., otherwise soem logs may still be buffered without being flushed before the app exits).
			Log.CloseAndFlush();
		};

		var s = new ServiceCollection()
			.AddApi()
			.AddCli()
			.AddConfiguration(platform)
			.AddLogging()
			.AddSingleton(platform)
			.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>()
			.AddSingleton<WtqHost>()
			.AddUI()
			.AddWtqCore();

		ConfigureServices(s);

		var p = s.BuildServiceProvider();

		return await new CommandBuilder()
			.Build(t => p.GetRequiredService(t))
			.Parse(args)
			.InvokeAsync()
			.NoCtx();
	}
}
