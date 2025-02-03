using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wtq.Services.UI;

namespace Wtq.Host.Base;

public class WtqHostBase
{
	public void Run(string[] args)
	{
		// Setup logging ASAP, so we can log stuff if initialization goes awry.
		Log.Configure();

		var log = Log.For<WtqHostBase>();


		AppDomain.CurrentDomain.ProcessExit += (_, _) =>
		{
			// appLifetime.StopApplication();
			Console.WriteLine("ProcessExit");
		};

		Console.CancelKeyPress += (_, _) =>
		{
			// appLifetime.StopApplication();
			Console.WriteLine("CancelKeyPress");
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
				})
				.AddCommandLine(args)
				.Build();

			WtqUIHostBuilder.Run(s =>
			{
				s
					.AddOptionsWithValidateOnStart<WtqOptions>()
					.Bind(config);

				s
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

	protected virtual void ConfigureServices(IServiceCollection services)
	{
		// Implemented by OS-specific implementations.
	}
}