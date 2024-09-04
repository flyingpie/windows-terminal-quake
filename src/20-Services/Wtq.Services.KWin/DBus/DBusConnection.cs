using Tmds.DBus;
using Wtq.Exceptions;
using Address = Tmds.DBus.Protocol.Address;
using Connection = Tmds.DBus.Protocol.Connection;

namespace Wtq.Services.KWin.DBus;

/// <inheritdoc/>
public class DBusConnection : IDBusConnection
{
	private readonly ILogger _log = Log.For<DBusConnection>();

	public DBusConnection()
	{
		var address = Address.Session;

		_log.LogInformation("Setting up DBus using address {Address}", address);

		if (string.IsNullOrWhiteSpace(address))
		{
			throw new WtqException("Could not determine address for session DBus.");
		}

		ClientConnection = new Connection(address);
		ServerConnection = new Tmds.DBus.Connection(address);
	}

	/// <inheritdoc/>
	public Connection ClientConnection { get; }

	/// <inheritdoc/>
	public Tmds.DBus.Connection ServerConnection { get; }

	/// <summary>
	/// Connects to DBus.
	/// </summary>
	public async Task StartAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Setting up DBus connections");

		var sw = Stopwatch.StartNew();
		await ClientConnection.ConnectAsync().NoCtx();
		_log.LogInformation("DBus client connection ready, took {Elapsed}", sw.Elapsed);

		sw.Restart();
		await ServerConnection.ConnectAsync().NoCtx();
		_log.LogInformation("DBus server connection ready, took {Elapsed}", sw.Elapsed);
	}

	/// <summary>
	/// Cleans up connections to DBus.
	/// </summary>
	public Task StopAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Cleaning up DBus connections");

		ClientConnection.Dispose();
		ServerConnection.Dispose();

		return Task.CompletedTask;
	}

	/// <inheritdoc/>
	public async Task RegisterServiceAsync(string serviceName, IDBusObject serviceObject)
	{
		Guard.Against.NullOrWhiteSpace(serviceName);
		Guard.Against.Null(serviceObject);

		_log.LogInformation("Registering DBus service with name '{ServiceName}', and object '{ServiceObject}'", serviceName, serviceObject);

		await ServerConnection.RegisterServiceAsync(serviceName).NoCtx();
		await ServerConnection.RegisterObjectAsync(serviceObject).NoCtx();
	}
}