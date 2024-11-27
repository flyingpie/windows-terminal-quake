using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Wtq.Services.UI;

namespace Wtq.Host.Base;

public class WtqHostBase
{
	public async Task RunAsync(string[] args)
	{
		// Setup logging ASAP, so we can log stuff if initialization goes awry.
		Utils.Log.Configure();

		var log = Utils.Log.For(typeof(WtqHostBase));

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
				})
				.AddCommandLine(args)
				.Build();

			await new HostBuilder()
				.ConfigureAppConfiguration(opt =>
				{
					opt.AddConfiguration(config);
				})
				.ConfigureServices(opt =>
				{
					opt
						.AddOptionsWithValidateOnStart<WtqOptions>()
						.Bind(config);

					opt
						.AddUI()

						// Utils
						.AddWtqCore();

					ConfigureServices(opt);
				})
				.UseSerilog()
				.Build()

				// Run!
				.RunAsync()
				.NoCtx();
		}
		catch (Exception ex)
		{
			log.LogError(ex, "Error running application: {Message}", ex.Message);
		}
	}

	protected virtual void ConfigureServices(IServiceCollection services)
	{
		// Implemented by OS-specific implementations.
	}
}