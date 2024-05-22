using Tmds.DBus;
using Address = Tmds.DBus.Protocol.Address;
using Connection = Tmds.DBus.Protocol.Connection;

namespace Wtq.Services.KWin.DBus;

[DBusInterface("wtq.thingy")]
public interface IThingy : IDBusObject
{
	Task<string> DoStuffAsync(string stuff);
}

public class Thingy : IThingy
{
	public static readonly ObjectPath Path = new("/wtq/thingy");

	public async Task<string> DoStuffAsync(string stuff)
	{
		Console.WriteLine($"Yeah!!! {stuff}");

		return "Response from thingy!";
	}

	public ObjectPath ObjectPath => Path;
}

public class KWinClient1
{
	public async Task StuffAsync()
	{
		// Server
		using var serverConn = new Tmds.DBus.Connection(Address.Session);
		await serverConn.ConnectAsync();

		await serverConn.RegisterServiceAsync("wtq.svc1");
		await serverConn.RegisterObjectAsync(new Thingy());

		// Client
		using var clientConn = new Connection(Address.Session);
		await clientConn.ConnectAsync();

		Console.WriteLine("Running");
		Console.WriteLine($"Connected to DBus {clientConn.UniqueName}");

		var srvc = new KWinService(clientConn, "org.kde.KWin");
		var scr = srvc.CreateScripting("/Scripting");

		var script1 =
			"""
			console.log('Before!');
			callDBus("wtq.svc1", "/wtq/thingy", "wtq.thingy", "DoStuff", "Wooooot");
			callDBus("wtq.svc1", "/wtq/thingy", "wtq.thingy", "DoStuff", JSON.stringify(workspace.clientList()));
			console.log('After!');
			""";

		await File.WriteAllTextAsync("/home/marco/tmp/my-script.js", script1);
		await scr.LoadScriptAsync("/home/marco/tmp/my-script.js", "plugin-name");
		await scr.StartAsync();
		await scr.UnloadScriptAsync("plugin-name");
	}

	public async Task DoStuffAsync()
	{
		
	}
}