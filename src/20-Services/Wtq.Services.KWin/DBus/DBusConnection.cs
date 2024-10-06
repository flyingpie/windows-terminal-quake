using Tmds.DBus;
using Wtq.Exceptions;
using Address = Tmds.DBus.Protocol.Address;
using Connection = Tmds.DBus.Protocol.Connection;

namespace Wtq.Services.KWin.DBus;

/// <inheritdoc/>
public sealed class DBusConnection : IDBusConnection
{
	private readonly ILogger _log = Log.For<DBusConnection>();

	private readonly Initializer _init;

	public DBusConnection()
		: this(Address.Session)
	{
	}

	public DBusConnection(string? address)
	{
		Guard.Against.NullOrWhiteSpace(address);

		_init = new(InitializeAsync);

		_log.LogInformation("Setting up DBus using address {Address}", address);

		ClientConnection = new Connection(address);
		ServerConnection = new Tmds.DBus.Connection(address);
	}

	/// <inheritdoc/>
	public Connection ClientConnection { get; }

	/// <inheritdoc/>
	public Tmds.DBus.Connection ServerConnection { get; }

	/// <summary>
	/// Cleans up connections to DBus.
	/// </summary>
	public void Dispose()
	{
		_log.LogInformation("Cleaning up DBus connections");

		ClientConnection.Dispose();
		ServerConnection.Dispose();

		_init.Dispose();
	}

	/// <inheritdoc/>
	public async Task RegisterServiceAsync(string serviceName, IDBusObject serviceObject)
	{
		Guard.Against.NullOrWhiteSpace(serviceName);
		Guard.Against.Null(serviceObject);

		await _init.InitializeAsync().NoCtx();

		_log.LogInformation("Registering DBus service with name '{ServiceName}', and object '{ServiceObject}'", serviceName, serviceObject);

		await ServerConnection.RegisterServiceAsync(serviceName).NoCtx();
		await ServerConnection.RegisterObjectAsync(serviceObject).NoCtx();
	}

	private async Task InitializeAsync()
	{
		_log.LogInformation("Setting up DBus connections");

		var sw = Stopwatch.StartNew();
		await ClientConnection.ConnectAsync().NoCtx();
		_log.LogInformation("DBus client connection ready, took {Elapsed}", sw.Elapsed);

		sw.Restart();
		await ServerConnection.ConnectAsync().NoCtx();
		_log.LogInformation("DBus server connection ready, took {Elapsed}", sw.Elapsed);
	}
}