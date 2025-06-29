using DeclarativeCommandLine.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Wtq.Services.API;
using Wtq.Services.CLI;
using Wtq.Services.UI;

namespace Wtq.Host.Base;

public class WtqHostBase
{
	public async Task RunAsync(string[] args)
	{
		// Setup logging ASAP, so we can log stuff if initialization goes awry.
		Log.Configure();

		if (args.Length == 0)
		{
			RunApp(args);
		}
		else
		{
			await RunCli(args).NoCtx();
		}
	}

	protected virtual void ConfigureServices(IServiceCollection services)
	{
		// Implemented by OS-specific implementations.
	}

	private void RunApp(string[] args)
	{
		var log = Log.For<WtqHostBase>();

		AppDomain.CurrentDomain.ProcessExit += (s, a) =>
		{
			log.LogInformation("Process exit");
			Log.CloseAndFlush();
		};

		try
		{
			// Find path to settings files (wtq.jsonc or similar).
			var pathToWtqConf = WtqOptionsPath.Instance.Path;

			// Write wtq.schema.json.
			WtqSchema.WriteFor(pathToWtqConf);

			// Load config file.
			var config = new ConfigurationBuilder()
				.SetBasePath(Path.GetDirectoryName(pathToWtqConf)!)
				.AddEnvironmentVariables()
				.AddJsonFile(f =>
				{
					f.ReloadOnChange = true;
					f.Optional = false;
					f.Path = Path.GetFileName(pathToWtqConf);
					f.OnLoadException = x =>
					{
						log.LogError(x.Exception, "Error loading configuration file '{File}': {Message}", pathToWtqConf, x.Exception.Message);
					};

					if (Os.IsSymlink(pathToWtqConf))
					{
						log.LogInformation("Settings file '{Path}' appears to be a symlink, switching to polling file watcher, as otherwise changes may not be detected", pathToWtqConf);

						f.FileProvider = new PhysicalFileProvider(Path.GetDirectoryName(pathToWtqConf)!)
						{
							UseActivePolling = true,
							UsePollingFileWatcher = true,
						};
					}
				})
				.AddCommandLine(args)
				.Build();

			WtqUIHostBuilder.Run(s =>
			{
				s
					.AddOptionsWithValidateOnStart<WtqOptions>()
					.PostConfigure(o => o.OnPostConfigure())
					.Bind(config);

				s
					.AddApi()
					.AddUI()
					.AddWtqCore();

				ConfigureServices(s);
			});
		}
		catch (Exception ex)
		{
			log.LogError(ex, "Error running application: {Message}", ex.Message);
		}
	}

	private async Task RunCli(string[] args) =>
		await new ServiceCollection()
			.AddCli()
			.BuildServiceProvider()
			.RunCliAsync(args)
			.NoCtx();
}