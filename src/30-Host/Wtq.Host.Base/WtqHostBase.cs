using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using Serilog;
using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Wtq.Configuration;
using Wtq.Services.UI;

namespace Wtq.Host.Base;

public class WtqHostBase
{
	public async Task RunAsync(string[] args)
	{
		// Setup logging ASAP, so we can log stuff if initialization goes awry.
		Utils.Log.Configure();

		var log = Utils.Log.For(typeof(WtqHostBase));

		var schema = JsonSchema.FromType<WtqOptions>(
			new SystemTextJsonSchemaGeneratorSettings()
			{
				SerializerOptions =
				{
					Converters =
					{
						new JsonStringEnumConverter(),
					},
				},
			});

		var schemaData = schema.ToJson(Formatting.Indented);

		File.WriteAllText("/home/marco/Downloads/wtq.schema.json", schemaData);

		var dbg = 2;

		// Configuration.
		var pathToWtqConf = WtqOptionsPath.Instance.Path;
		var config = new ConfigurationBuilder()
			.SetBasePath(Path.GetDirectoryName(pathToWtqConf)!)
			.AddJsonFile(
				f =>
				{
					f.ReloadOnChange = true;
					f.Optional = false;
					f.Path = Path.GetFileName(pathToWtqConf);
					f.OnLoadException = x =>
					{
						log.LogError(x.Exception, "Error loading configuration file '{File}': {Message}", pathToWtqConf, x.Exception.Message);
						Console.WriteLine($"Error loading configuration file '{pathToWtqConf}': {x.Exception.Message}");

						// MessageBox.Show($"Error loading configuration file '{pathToWtqConf}': {x.Exception.Message}");
					};
				})
			.Build();

		_host = new HostBuilder()
			.ConfigureAppConfiguration(opt => { opt.AddConfiguration(config); })
			.ConfigureServices(
				opt =>
				{
					opt
						.AddOptionsWithValidateOnStart<WtqOptions>()
						.Bind(config);

					opt
						.AddUI()

						// Utils
						.AddWtqCore();
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