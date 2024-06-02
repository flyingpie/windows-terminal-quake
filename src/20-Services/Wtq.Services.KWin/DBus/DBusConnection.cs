using Microsoft.Extensions.Hosting;
using Tmds.DBus;
using Address = Tmds.DBus.Protocol.Address;
using Connection = Tmds.DBus.Protocol.Connection;

namespace Wtq.Services.KWin.DBus;

internal class DBusConnection : IDBusConnection
{
	public DBusConnection()
	{
		ClientConnection = new Connection(Address.Session);
		ServerConnection = new Tmds.DBus.Connection(Address.Session);
	}

	public Connection ClientConnection { get; }

	public Tmds.DBus.Connection ServerConnection { get; }

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await ClientConnection.ConnectAsync().ConfigureAwait(false);
		await ServerConnection.ConnectAsync().ConfigureAwait(false);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		ClientConnection.Dispose();
		ServerConnection.Dispose();

		return Task.CompletedTask;
	}

	public async Task RegisterServiceAsync(string serviceName, IDBusObject serviceObject)
	{
		await ServerConnection.RegisterServiceAsync(serviceName).ConfigureAwait(false);
		await ServerConnection.RegisterObjectAsync(serviceObject).ConfigureAwait(false);
	}
}