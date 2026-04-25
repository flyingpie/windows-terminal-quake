using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO.Pipes;
using System.Net.Sockets;
using Wtq.Host.Base.Commands;
using Wtq.Host.Base.Commands.Apps;
using Wtq.Services;

namespace Wtq.Host.Base;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCli(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddScoped<AppRootCommand>()
			.AddScoped<AppsCommand>()
			.AddScoped<CloseCommand>()
			.AddScoped<GuiCommand>()
			.AddScoped<InfoCommand>()
			.AddScoped<ListCommand>()
			.AddScoped<OpenCommand>()
			.AddSingleton<HttpClient>(p =>
			{
				var opts = p.GetRequiredService<IOptions<WtqOptions>>().Value;
				var platform = p.GetRequiredService<IPlatformService>();
				var urls = opts.Api.Urls ?? platform.DefaultApiUrls;
				var url = new Uri(urls.First());

				var handler = new SocketsHttpHandler();

				if (url.Host.Equals("pipe", StringComparison.OrdinalIgnoreCase))
				{
					handler.ConnectCallback = WindowsNamedPipe();
				}
				else if (url.Host.Equals("unix", StringComparison.OrdinalIgnoreCase))
				{
					handler.ConnectCallback = UnixSocket(url.AbsolutePath);
				}

				return new HttpClient(handler)
				{
					BaseAddress = new("http://localhost"),
				};
			});
	}

	private static Func<SocketsHttpConnectionContext, CancellationToken, ValueTask<Stream>> UnixSocket(string socketPath) =>
		async (_, ct) =>
		{
			var log = Log.For(nameof(UnixSocket));
			log.LogDebug("Connecting to Unix socket at '{SocketPath}'", socketPath);

			// Define the type of socket we want, i.e. a UDS stream-oriented socket
			var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);

			// Create a UDS endpoint using the socket path
			var endpoint = new UnixDomainSocketEndPoint(socketPath);

			// Connect to the server!
			await socket.ConnectAsync(endpoint, ct).NoCtx();

			// Wrap the socket in a NetworkStream and return it
			// Setting ownsSocket: true means the NetworkStream will
			// close and dispose the Socket when the stream is disposed
			return new NetworkStream(socket, ownsSocket: true);
		};

	private static Func<SocketsHttpConnectionContext, CancellationToken, ValueTask<Stream>> WindowsNamedPipe() =>
		async (_, ct) =>
		{
			var stream = new NamedPipeClientStream(
				serverName: ".",
				pipeName: "wtq",
				direction: PipeDirection.InOut,
				options: PipeOptions.Asynchronous);

			await stream.ConnectAsync(ct).NoCtx();

			return stream;
		};
}