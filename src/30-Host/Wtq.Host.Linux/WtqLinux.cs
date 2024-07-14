using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Services;
using Wtq.Services.KWin;
using Wtq.Utils;

namespace Wtq.Host.Linux;

public class WtqLinux
{
	private readonly IHost _host;

	public WtqLinux()
	{
		var log = Utils.Log.For(typeof(Program));

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
					.AddWtqCore()

					// Platform-specific.
					.AddKWin();
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
}