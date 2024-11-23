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
	private readonly IHost _host;

	public WtqHostBase()
	{
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

				ConfigureServices(opt);
			})
			.UseSerilog()
			.Build();
	}

	public async Task RunAsync()
	{
		try
		{
			await _host
				.RunAsync()
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error running application: {ex}");

			// MessageBox.Show($"Error running application: {ex}", "Error starting WTQ");
		}
	}

	protected virtual void ConfigureServices(IServiceCollection services)
	{
	}
}