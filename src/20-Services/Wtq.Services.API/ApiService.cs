using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Wtq.Configuration;

namespace Wtq.Services.HttpApi;

/// <summary>
/// Hosts the HTTP API using Kestel.
/// </summary>
public sealed class ApiService : WtqHostedService
{
	private readonly IOptions<WtqOptions> _opts;
	private readonly ILogger _log = Log.For<ApiService>();

	private readonly IWtqAppRepo _appRepo;
	private readonly IWtqBus _bus;
	private readonly CancellationTokenSource _cts = new();

	private WebApplication? _app;

	public ApiService(
		IOptions<WtqOptions> opts,
		IWtqAppRepo appRepo,
		IWtqBus bus)
	{
		_opts = Guard.Against.Null(opts);
		_appRepo = Guard.Against.Null(appRepo);
		_bus = Guard.Against.Null(bus);
	}

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

		foreach (var u in opt.Urls)
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

		builder.WebHost.UseUrls([.. opt.Urls]);

		builder.Services.AddControllers();
		builder.Services.AddOpenApi();

		builder.Services.AddSingleton(_bus);
		builder.Services.AddSingleton(_appRepo);

		_app = builder.Build();

		_app.MapControllers();
		_app.MapOpenApi();
		_app.MapScalarApiReference("/");

		_ = _app.RunAsync(_cts.Token);

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