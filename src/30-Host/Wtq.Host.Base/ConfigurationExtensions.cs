using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Wtq.Services;

namespace Wtq.Host.Base;

public static class ConfigurationExtensions
{
	public static IServiceCollection AddConfiguration(this IServiceCollection services, IPlatformService platform, string[] args)
	{
		Guard.Against.Null(services);
		Guard.Against.Null(platform);
		Guard.Against.Null(args);

		var config = CreateConfigurationRoot(platform, args);

		services
			.AddOptionsWithValidateOnStart<WtqOptions>()
			.PostConfigure(o => o.OnPostConfigure())
			.Bind(config);

		return services;
	}

	private static IConfiguration CreateConfigurationRoot(IPlatformService platform, string[] args)
	{
		Guard.Against.Null(platform);
		Guard.Against.Null(args);

		var log = Log.For(nameof(CreateConfigurationRoot));

		// Find path to settings files (wtq.jsonc or similar).
		var pathToWtqConf = platform.PathToWtqConf;

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

				if (!platform.ShouldUsePollingFileWatcherForPath(pathToWtqConf))
				{
					return;
				}

				log.LogInformation("Settings file '{Path}' appears to be a symlink, switching to polling file watcher, as otherwise changes may not be detected", pathToWtqConf);

				f.FileProvider = new PhysicalFileProvider(Path.GetDirectoryName(pathToWtqConf)!)
				{
					UseActivePolling = true,
					UsePollingFileWatcher = true,
				};
			})
			.AddCommandLine(args)
			.Build();

		return config;
	}
}