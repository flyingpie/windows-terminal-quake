using Tmds.DBus;
using Address = Tmds.DBus.Protocol.Address;
using Connection = Tmds.DBus.Protocol.Connection;

namespace Wtq.Services.KWin.DBus;

/// <inheritdoc/>
internal sealed class DBusConnection : IDBusConnection
{
	private readonly ILogger _log = Log.For<DBusConnection>();

	private readonly Initializer _init;

	private DBus.KWinService? _kwinService;
	private DBus.KWin? _kwin;
	private DBus.Scripting? _scripting;

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

	public async Task<DBus.KWinService> GetKWinServiceAsync()
	{
		await _init.InitializeAsync().NoCtx();

		return _kwinService ??= new KWinService(ClientConnection, "org.kde.KWin");
	}

	public async Task<DBus.KWin> GetKWinAsync()
	{
		await _init.InitializeAsync().NoCtx();

		return _kwin ??= (await GetKWinServiceAsync().NoCtx()).CreateKWin("/KWin");
	}

	public async Task<DBus.Scripting> GetScriptingAsync()
	{
		await _init.InitializeAsync().NoCtx();

		return _scripting ??= (await GetKWinServiceAsync().NoCtx()).CreateScripting("/Scripting");
	}

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