using Microsoft.Extensions.Hosting;
using Tmds.DBus;
using Address = Tmds.DBus.Protocol.Address;
using Connection = Tmds.DBus.Protocol.Connection;

namespace Wtq.Services.KWin.DBus;

/// <inheritdoc cref="IDBusConnection"/>
internal sealed class DBusConnection : IHostedService, IDBusConnection
{
	private readonly ILogger _log = Log.For<DBusConnection>();

	/// <summary>
	/// Client connection, used to send requests to DBus.
	/// </summary>
	private readonly Connection _clientConnection;

	/// <summary>
	/// Server connection, used to register DBus objects.
	/// </summary>
	private readonly Tmds.DBus.Connection _serverConnection;

	private DBus.Generated.KWinService? _kwinService;
	private DBus.Generated.KWin? _kwin;
	private DBus.Generated.Scripting? _scripting;

	public DBusConnection()
		: this(Address.Session)
	{
	}

	public DBusConnection(string? address)
	{
		Guard.Against.NullOrWhiteSpace(address);

		_log.LogInformation("Setting up DBus using address {Address}", address);

		_clientConnection = new Connection(address);
		_serverConnection = new Tmds.DBus.Connection(address);
	}

	public int InitializePriority => 20;

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Setting up DBus connections");

		var sw = Stopwatch.StartNew();
		await _clientConnection.ConnectAsync().NoCtx();
		_log.LogInformation("DBus client connection ready, took {Elapsed}", sw.Elapsed);

		sw.Restart();
		await _serverConnection.ConnectAsync().NoCtx();
		_log.LogInformation("DBus server connection ready, took {Elapsed}", sw.Elapsed);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public async Task<DBus.Generated.KWinService> GetKWinServiceAsync()
	{
		return _kwinService ??= new DBus.Generated.KWinService(_clientConnection, "org.kde.KWin");
	}

	public async Task<DBus.Generated.KWin> GetKWinAsync()
	{
		return _kwin ??= (await GetKWinServiceAsync().NoCtx()).CreateKWin("/KWin");
	}

	public async Task<DBus.Generated.Scripting> GetScriptingAsync()
	{
		return _scripting ??= (await GetKWinServiceAsync().NoCtx()).CreateScripting("/Scripting");
	}

	/// <summary>
	/// Cleans up connections to DBus.
	/// </summary>
	public void Dispose()
	{
		_log.LogInformation("Cleaning up DBus connections");

		_clientConnection.Dispose();
		_serverConnection.Dispose();
	}

	/// <inheritdoc/>
	public async Task RegisterServiceAsync(string serviceName, IDBusObject serviceObject)
	{
		Guard.Against.NullOrWhiteSpace(serviceName);
		Guard.Against.Null(serviceObject);

		_log.LogInformation("Registering DBus service with name '{ServiceName}', and object '{ServiceObject}'", serviceName, serviceObject);

		await _serverConnection.RegisterServiceAsync(serviceName).NoCtx();
		await _serverConnection.RegisterObjectAsync(serviceObject).NoCtx();
	}
}