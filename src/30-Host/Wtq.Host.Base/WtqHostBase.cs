using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using Wtq.Configuration;
using Wtq.Utils;

namespace Wtq.Host.Base;

public class WtqHostBase
{
	private readonly IHost _host;

	public WtqHostBase()
	{
		var log = Utils.Log.For(typeof(WtqHostBase));

		// Configuration.
		var pathToWtqConf = WtqOptionsPath.Instance.Path;
		var config = new ConfigurationBuilder()
			.SetBasePath(Path.GetDirectoryName(pathToWtqConf)!)
			.AddJsonFile(f =>
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

			// .UseServiceProviderFactory((x) => new AsyncServiceProviderFactory())

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

					// Utils
					.AddWtqCore();

				ConfigureServices(opt);

				opt
					.AddAsyncInitializable();
			})
			.UseSerilog()
			.Build();
	}

	public async Task RunAsync()
	{
		try
		{
			await _host.Services.InitializeAsync().NoCtx();

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