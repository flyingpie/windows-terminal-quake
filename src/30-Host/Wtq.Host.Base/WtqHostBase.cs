using Microsoft.Extensions.DependencyInjection;
using Wtq.Services;
using Wtq.Services.API;
using Wtq.Services.CLI;
using Wtq.Services.CLI.Commands;
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

		if (args.Length == 0)
		{
			RunApp(platform, args);
		}
		else
		{
			await RunCliAsync(platform, args).NoCtx();
		}
	}

	protected abstract IPlatformService CreatePlatformService();

	protected virtual void ConfigureServices(IServiceCollection services)
	{
		// Implemented by OS-specific implementations.
	}

	/// <summary>
	/// This is called whenever the app is being run in CLI mode (i.e. with command line arguments).
	/// </summary>
	private static async Task<int> RunCliAsync(IPlatformService platform, string[] args)
	{
		var p = new ServiceCollection()
			.AddSingleton(platform)
			.AddConfiguration(platform, args)
			.AddCli()
			.BuildServiceProvider();

		return await new CommandBuilder()
			.Build(t => p.GetRequiredService(t))
			.Parse(args)
			.InvokeAsync()
			.NoCtx();
	}

	/// <summary>
	/// This is called when the app is being run in GUI mode (i.e. without command line arguments).
	/// </summary>
	private void RunApp(IPlatformService platform, string[] args)
	{
		var log = Log.For<WtqHostBase>();

		AppDomain.CurrentDomain.ProcessExit += (s, a) =>
		{
			log.LogInformation("Process exit");
			Log.CloseAndFlush();
		};

		try
		{
			WtqUIHostBuilder.Run(s =>
			{
				s
					.AddSingleton(platform)
					.AddConfiguration(platform, args)
					.AddApi()
					.AddUI()
					.AddWtqCore()
				;

				ConfigureServices(s);
			}, gui: false);
		}
		catch (Exception ex)
		{
			log.LogError(ex, "Error running application: {Message}", ex.Message);
		}
	}
}