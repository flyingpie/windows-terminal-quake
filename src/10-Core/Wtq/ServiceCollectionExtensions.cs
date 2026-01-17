using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Wtq.Services;

namespace Wtq;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddHostedServiceSingleton<TImplementation>(
		this IServiceCollection services)
		where TImplementation : class, IHostedService
	{
		Guard.Against.Null(services);

		return services
			.AddSingleton<TImplementation>()
			.AddHostedService(p => p.GetRequiredService<TImplementation>());
	}

	public static IServiceCollection AddHostedServiceSingleton<TService, TImplementation>(
		this IServiceCollection services)
		where TImplementation : class, IHostedService, TService
		where TService : class
	{
		Guard.Against.Null(services);

		return services
			.AddSingleton<TService, TImplementation>()
			.AddHostedService(p => (TImplementation)p.GetRequiredService<TService>());
	}

	public static IServiceCollection AddConfiguration(this IServiceCollection services, IPlatformService platform)
	{
		Guard.Against.Null(services);
		Guard.Against.Null(platform);

		var config = CreateConfigurationRoot(platform);

		services
			.AddOptionsWithValidateOnStart<WtqOptions>()
			.PostConfigure(o => o.OnPostConfigure())
			.Bind(config);

		return services;
	}

	private static IConfiguration CreateConfigurationRoot(IPlatformService platform)
	{
		Guard.Against.Null(platform);

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
			.Build();

		return config;
	}

	public static IServiceCollection AddWtqCore(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services

			// Utils
			.AddSingleton<IWtqTween, WtqTween>()

			// Core App Logic
			.AddSingleton<IWtqAppToggleService, WtqAppToggleService>()
			.AddSingleton<IWtqBus, WtqBus>()
			.AddSingleton<IWtqOptionsSaveService, WtqOptionsSaveService>()
			.AddSingleton<IWtqTargetScreenRectProvider, WtqTargetScreenRectProvider>()
			.AddSingleton<IWtqWindowRectProvider, WtqWindowRectProvider>()
			.AddSingleton<IWtqWindowResolver, WtqWindowResolver>()
			.AddSingleton<TrayIconUtil>()

			.AddHostedService<WtqEventHookHandler>()
			.AddHostedService<WtqFocusTracker>()
			.AddHostedService<WtqHotkeyRoutingService>()
			.AddHostedService<WtqService>()
			.AddHostedServiceSingleton<IWtqAppRepo, WtqAppRepo>();
	}
}