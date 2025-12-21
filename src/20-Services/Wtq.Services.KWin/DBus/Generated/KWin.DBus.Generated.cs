#pragma warning disable

using System;
using Tmds.DBus.Protocol;
using SafeHandle = System.Runtime.InteropServices.SafeHandle;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wtq.Services.KWin.DBus.Generated;

partial class KWin : KWinObject
{
	private const string __Interface = "org.kde.KWin";
	public KWin(KWinService service, ObjectPath path) : base(service, path)
	{ }
	public Task<bool> SetCurrentDesktopAsync(int desktop)
	{
		return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
		MessageBuffer CreateMessage()
		{
			var writer = this.Connection.GetMessageWriter();
			writer.WriteMethodCallHeader(
				destination: Service.Destination,
				path: Path,
				@interface: __Interface,
				signature: "i",
				member: "setCurrentDesktop");
			writer.WriteInt32(desktop);
			return writer.CreateMessage();
		}
	}
	public Task<int> CurrentDesktopAsync()
	{
		return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (KWinObject)s!), this);
		MessageBuffer CreateMessage()
		{
			var writer = this.Connection.GetMessageWriter();
			writer.WriteMethodCallHeader(
				destination: Service.Destination,
				path: Path,
				@interface: __Interface,
				member: "currentDesktop");
			return writer.CreateMessage();
		}
	}
	public Task<string> SupportInformationAsync()
	{
		return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_s(m, (KWinObject)s!), this);
		MessageBuffer CreateMessage()
		{
			var writer = this.Connection.GetMessageWriter();
			writer.WriteMethodCallHeader(
				destination: Service.Destination,
				path: Path,
				@interface: __Interface,
				member: "supportInformation");
			return writer.CreateMessage();
		}
	}
}
partial class Scripting : KWinObject
{
	private const string __Interface = "org.kde.kwin.Scripting";
	public Scripting(KWinService service, ObjectPath path) : base(service, path)
	{ }
	public Task StartAsync()
	{
		return this.Connection.CallMethodAsync(CreateMessage());
		MessageBuffer CreateMessage()
		{
			var writer = this.Connection.GetMessageWriter();
			writer.WriteMethodCallHeader(
				destination: Service.Destination,
				path: Path,
				@interface: __Interface,
				member: "start");
			return writer.CreateMessage();
		}
	}
	public Task<int> LoadScriptAsync(string filePath, string pluginName)
	{
		return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (KWinObject)s!), this);
		MessageBuffer CreateMessage()
		{
			var writer = this.Connection.GetMessageWriter();
			writer.WriteMethodCallHeader(
				destination: Service.Destination,
				path: Path,
				@interface: __Interface,
				signature: "ss",
				member: "loadScript");
			writer.WriteString(filePath);
			writer.WriteString(pluginName);
			return writer.CreateMessage();
		}
	}
	public Task<bool> IsScriptLoadedAsync(string pluginName)
	{
		return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
		MessageBuffer CreateMessage()
		{
			var writer = this.Connection.GetMessageWriter();
			writer.WriteMethodCallHeader(
				destination: Service.Destination,
				path: Path,
				@interface: __Interface,
				signature: "s",
				member: "isScriptLoaded");
			writer.WriteString(pluginName);
			return writer.CreateMessage();
		}
	}
	public Task<bool> UnloadScriptAsync(string pluginName)
	{
		return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
		MessageBuffer CreateMessage()
		{
			var writer = this.Connection.GetMessageWriter();
			writer.WriteMethodCallHeader(
				destination: Service.Destination,
				path: Path,
				@interface: __Interface,
				signature: "s",
				member: "unloadScript");
			writer.WriteString(pluginName);
			return writer.CreateMessage();
		}
	}
}
partial class Component : KWinObject
{
	private const string __Interface = "org.kde.kglobalaccel.Component";
	public Component(KWinService service, ObjectPath path) : base(service, path)
	{ }
	public Task<bool> CleanUpAsync()
	{
		return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
		MessageBuffer CreateMessage()
		{
			var writer = this.Connection.GetMessageWriter();
			writer.WriteMethodCallHeader(
				destination: Service.Destination,
				path: Path,
				@interface: __Interface,
				member: "cleanUp");
			return writer.CreateMessage();
		}
	}
	public Task<string[]> ShortcutNamesAsync()
	{
		return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_as(m, (KWinObject)s!), this);
		MessageBuffer CreateMessage()
		{
			var writer = this.Connection.GetMessageWriter();
			writer.WriteMethodCallHeader(
				destination: Service.Destination,
				path: Path,
				@interface: __Interface,
				member: "shortcutNames");
			return writer.CreateMessage();
		}
	}
}
partial class KGlobalAccel : KWinObject
{
	private const string __Interface = "org.kde.KGlobalAccel";
	public KGlobalAccel(KWinService service, ObjectPath path) : base(service, path)
	{ }
	public Task<bool> UnregisterAsync(string componentUnique, string shortcutUnique)
	{
		return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
		MessageBuffer CreateMessage()
		{
			var writer = this.Connection.GetMessageWriter();
			writer.WriteMethodCallHeader(
				destination: Service.Destination,
				path: Path,
				@interface: __Interface,
				signature: "ss",
				member: "unregister");
			writer.WriteString(componentUnique);
			writer.WriteString(shortcutUnique);
			return writer.CreateMessage();
		}
	}
}
partial class KWinService
{
	public Tmds.DBus.Protocol.Connection Connection { get; }
	public string Destination { get; }
	public KWinService(Tmds.DBus.Protocol.Connection connection, string destination)
		=> (Connection, Destination) = (connection, destination);
	public KWin CreateKWin(string path) => new KWin(this, path);
	public Scripting CreateScripting(string path) => new Scripting(this, path);
	public Component CreateComponent(string path) => new Component(this, path);
	public KGlobalAccel CreateKGlobalAccel(string path) => new KGlobalAccel(this, path);
}
class KWinObject
{
	public KWinService Service { get; }
	public ObjectPath Path { get; }
	protected Tmds.DBus.Protocol.Connection Connection => Service.Connection;
	protected KWinObject(KWinService service, ObjectPath path)
		=> (Service, Path) = (service, path);
	protected static bool ReadMessage_b(Message message, KWinObject _)
	{
		var reader = message.GetBodyReader();
		return reader.ReadBool();
	}
	protected static string ReadMessage_s(Message message, KWinObject _)
	{
		var reader = message.GetBodyReader();
		return reader.ReadString();
	}
	protected static int ReadMessage_i(Message message, KWinObject _)
	{
		var reader = message.GetBodyReader();
		return reader.ReadInt32();
	}
	protected static string[] ReadMessage_as(Message message, KWinObject _)
	{
		var reader = message.GetBodyReader();
		return reader.ReadArrayOfString();
	}
}