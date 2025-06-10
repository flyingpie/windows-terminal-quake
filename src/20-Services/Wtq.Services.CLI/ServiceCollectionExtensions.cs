using DeclarativeCommandLine.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;
using Wtq.Services.CLI.Commands.Apps;

namespace Wtq.Services.CLI;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddCli(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddDeclarativeCommandLine()
			.AddAllCommandsFromAssemblies<AppsCommand>()
			.AddSingleton<HttpClient>(p =>
			{
				var handler = new SocketsHttpHandler()
				{
					ConnectCallback = async (ctx, ct) =>
					{
						var socketPath = "/tmp/wtq.sock";

						// Define the type of socket we want, i.e. a UDS stream-oriented socket
						var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);

						// Create a UDS endpoint using the socket path
						var endpoint = new UnixDomainSocketEndPoint(socketPath);

						// Connect to the server!
						await socket.ConnectAsync(endpoint, ct);

						// Wrap the socket in a NetworkStream and return it
						// Setting ownsSocket: true means the NetworkStream will
						// close and dispose the Socket when the stream is disposed
						return new NetworkStream(socket, ownsSocket: true);
					},
				};

				return new HttpClient(handler)
				{
					BaseAddress = new("http://localhost"),
				};
			});
	}
}