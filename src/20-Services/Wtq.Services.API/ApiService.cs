using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Wtq.Configuration;

namespace Wtq.Services.API;

/// <summary>
/// Hosts the HTTP API using Kestel.
/// </summary>
public sealed class ApiService(
	IOptions<WtqOptions> opts,
	IPlatformService platformService,
	IWtqAppRepo appRepo,
	IWtqBus bus)
	: WtqHostedService
{
	private readonly ILogger _log = Log.For<ApiService>();

	private readonly IOptions<WtqOptions> _opts = Guard.Against.Null(opts);
	private readonly IPlatformService _platformService = Guard.Against.Null(platformService);
	private readonly IWtqAppRepo _appRepo = Guard.Against.Null(appRepo);
	private readonly IWtqBus _bus = Guard.Against.Null(bus);

	private readonly CancellationTokenSource _cts = new();

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		// Check whether we need to enable the API.
		var opt = _opts.Value.Api ?? new WtqApiOptions();
		if (!opt.Enable)
		{
			_log.LogDebug("API is DISABLED");
			return Task.CompletedTask;
		}

		var builder = WebApplication.CreateBuilder();

		var urls = opt.Urls ?? _platformService.DefaultApiUrls;

		_log.LogInformation("Hosting API on url(s) {Urls}", string.Join(", ", urls));

		foreach (var u in urls)
		{
			// Parse the url as a Uri.
			if (!Uri.TryCreate(u, UriKind.Absolute, out var uri))
			{
				_log.LogWarning("Could not parse listen url '{Url}' as an uri", u);
				continue;
			}

			// If this is a Unix socket, see if the target file already exists.
			// This could mean that WTQ is already running, or that it didn't shut down gracefully.
			// In any case, we're removing the socket file if it already exists. Preventing multiple instances trampling each other should be handled differently.
			if (u.StartsWith("http://unix:/", StringComparison.OrdinalIgnoreCase) && File.Exists(uri.AbsolutePath))
			{
				_log.LogWarning("Unix socket at path '{Path}' already exists, deleting", uri.AbsolutePath);
				File.Delete(uri.AbsolutePath);
			}
		}

		builder.WebHost.UseUrls([.. urls]);

		builder.Services
			.AddLogging(c => c
				.ClearProviders()
				.AddProvider(Log.Provider))

			// Add MVC controllers.
			.AddControllers()

			// Explicitly add the current namespace, since it may not be picked up otherwise
			// (due to the controllers being outside the entry assembly).
			.AddApplicationPart(typeof(ApiService).Assembly);

		builder.Services.AddOpenApi();

		builder.Services.AddSingleton(_bus);
		builder.Services.AddSingleton(_appRepo);

		var app = builder.Build();

		app.MapControllers();
		app.MapOpenApi();
		app.MapScalarApiReference("/");

		_ = app.RunAsync(_cts.Token);

		return Task.CompletedTask;
	}

	protected override async Task OnStopAsync(CancellationToken cancellationToken)
	{
		await _cts.CancelAsync().NoCtx();
	}

	protected override ValueTask OnDisposeAsync()
	{
		_cts.Dispose();

		return ValueTask.CompletedTask;
	}
}