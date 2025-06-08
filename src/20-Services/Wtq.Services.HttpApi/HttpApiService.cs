using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace Wtq.Services.HttpApi;

public sealed class HttpApiService : WtqHostedService
{
	private readonly ILogger _log = Log.For<HttpApiService>();

	private readonly IWtqAppRepo _appRepo;
	private readonly IWtqBus _bus;

	private WebApplication? _app;
	private CancellationTokenSource _cts = new();

	public HttpApiService(
		IWtqAppRepo appRepo,
		IWtqBus bus)
	{
		_appRepo = Guard.Against.Null(appRepo);
		_bus = Guard.Against.Null(bus);
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		var builder = WebApplication.CreateBuilder();

		builder.WebHost.UseUrls("http://127.0.0.1:8998");
		// builder.WebHost.ConfigureKestrel(opt =>
		// {
		// 	if (Os.IsLinux)
		// 	{
		// 		var socketPath = Path.Combine(Path.GetTempPath(), "wtq.sock");
		// 		Console.WriteLine($"Listening on pipe '{socketPath}'");
		// 		opt.ListenUnixSocket(socketPath);
		// 	}
		//
		// 	else if (Os.IsWindows)
		// 	{
		// 		opt.ListenNamedPipe("wtq");
		// 	}
		//
		// 	else
		// 	{
		// 		// TODO
		// 	}
		// });

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

	protected override Task OnStopAsync(CancellationToken cancellationToken)
	{
		_cts.Cancel();

		return Task.CompletedTask;
	}
}