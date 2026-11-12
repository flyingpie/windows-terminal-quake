// using Tmds.DBus.Protocol;
// using Wtq.Services.KWin.Dto;
//
// namespace Wtq.Host.Linux.E2ETest;
//
// /// <summary>
// /// Talks to KWin (and kglobalaccel) over DBus, independently of WTQ itself.<br/>
// /// Used to observe what WTQ does to windows, and to trigger WTQ hotkeys without simulating key presses.
// /// </summary>
// public sealed class KWinTestClient : IDisposable
// {
// 	private const string KWinService = "org.kde.KWin";
// 	private const string KGlobalAccelService = "org.kde.kglobalaccel";
//
// 	private readonly DBusConnection _conn = new(DBusAddress.Session!);
//
// 	public void Dispose() => _conn.Dispose();
//
// 	public async Task ConnectAsync() => await _conn.ConnectAsync();
//
// 	/// <summary>
// 	/// Screen rectangles, in the same order WTQ sees them (WTQ parses the same KWin support information).
// 	/// </summary>
// 	public async Task<Rectangle[]> GetScreenRectsAsync()
// 	{
// 		var supportInfo = await CallAsync(
// 			KWinService,
// 			"/KWin",
// 			"org.kde.KWin",
// 			"supportInformation",
// 			[],
// 			m => m.GetBodyReader().ReadString()
// 		);
//
// 		return KWinSupportInformation.Parse(supportInfo).Screens.Select(s => s.Geometry).ToArray();
// 	}
//
// 	/// <summary>
// 	/// Returns the internal ids (UUIDs) of windows whose caption matches the specified <paramref name="query"/>.<br/>
// 	/// Uses the KRunner "windows" plugin, since KWin doesn't expose a plain "list windows" DBus method.
// 	/// </summary>
// 	public async Task<ICollection<string>> FindWindowIdsAsync(string query) =>
// 		await CallAsync(
// 			KWinService,
// 			"/WindowsRunner",
// 			"org.kde.krunner1",
// 			"Match",
// 			[query],
// 			m =>
// 			{
// 				// Signature: a(sssida{sv}), where the match id looks like "0_{uuid}".
// 				var reader = m.GetBodyReader();
// 				var res = new List<string>();
// 				var end = reader.ReadArrayStart(DBusType.Struct);
// 				while (reader.HasNext(end))
// 				{
// 					reader.AlignStruct();
// 					var matchId = reader.ReadString();
// 					_ = reader.ReadString(); // Text (caption)
// 					_ = reader.ReadString(); // Icon
// 					_ = reader.ReadInt32(); // Category relevance (int or uint depending on Plasma version, both 4 bytes)
// 					_ = reader.ReadDouble(); // Relevance
// 					_ = reader.ReadDictionaryOfStringToVariantValue(); // Properties
//
// 					var idx = matchId.IndexOf('{', StringComparison.Ordinal);
// 					if (idx >= 0)
// 					{
// 						res.Add(matchId[idx..]);
// 					}
// 				}
//
// 				return res;
// 			}
// 		);
//
// 	/// <summary>
// 	/// Returns info (including frame geometry) of the window with the specified <paramref name="windowId"/>, or null if no such window exists.
// 	/// </summary>
// 	public async Task<WindowInfo?> GetWindowInfoAsync(string windowId)
// 	{
// 		var dict = await CallAsync(
// 			KWinService,
// 			"/KWin",
// 			"org.kde.KWin",
// 			"getWindowInfo",
// 			[windowId],
// 			m => m.GetBodyReader().ReadDictionaryOfStringToVariantValue()
// 		);
//
// 		// KWin returns an empty map for unknown windows.
// 		if (!dict.TryGetValue("x", out var x))
// 		{
// 			return null;
// 		}
//
// 		return new WindowInfo(
// 			Pid: dict["pid"].GetInt32(),
// 			Caption: dict["caption"].GetString(),
// 			Rect: new Rectangle(
// 				(int)Math.Round(x.GetDouble()),
// 				(int)Math.Round(dict["y"].GetDouble()),
// 				(int)Math.Round(dict["width"].GetDouble()),
// 				(int)Math.Round(dict["height"].GetDouble())
// 			)
// 		);
// 	}
//
// 	/// <summary>
// 	/// Names of all shortcuts registered by KWin (and thus by KWin scripts, like the one WTQ loads).
// 	/// </summary>
// 	public async Task<string[]> GetKWinShortcutNamesAsync() =>
// 		await CallAsync(
// 			KGlobalAccelService,
// 			"/component/kwin",
// 			"org.kde.kglobalaccel.Component",
// 			"shortcutNames",
// 			[],
// 			m => m.GetBodyReader().ReadArrayOfString()
// 		);
//
// 	/// <summary>
// 	/// Fires a KWin shortcut, as if its key sequence was pressed.
// 	/// </summary>
// 	public async Task InvokeKWinShortcutAsync(string name) =>
// 		await CallAsync(
// 			KGlobalAccelService,
// 			"/component/kwin",
// 			"org.kde.kglobalaccel.Component",
// 			"invokeShortcut",
// 			[name],
// 			_ => true
// 		);
//
// 	public async Task<bool> UnregisterKWinShortcutAsync(string name) =>
// 		await CallAsync(
// 			KGlobalAccelService,
// 			"/kglobalaccel",
// 			"org.kde.KGlobalAccel",
// 			"unregister",
// 			["kwin", name],
// 			m => m.GetBodyReader().ReadBool()
// 		);
//
// 	/// <summary>
// 	/// Calls a DBus method that only takes string arguments (which is all we need here).<br/>
// 	/// Note that <c>MessageWriter</c> is a struct, so it must not be passed around by value while writing.
// 	/// </summary>
// 	private Task<T> CallAsync<T>(
// 		string service,
// 		string path,
// 		string iface,
// 		string member,
// 		string[] args,
// 		Func<Message, T> read
// 	)
// 	{
// 		var writer = _conn.GetMessageWriter();
// 		writer.WriteMethodCallHeader(
// 			destination: service,
// 			path: path,
// 			@interface: iface,
// 			signature: args.Length > 0 ? new string('s', args.Length) : null,
// 			member: member
// 		);
//
// 		foreach (var arg in args)
// 		{
// 			writer.WriteString(arg);
// 		}
//
// 		return _conn.CallMethodAsync(writer.CreateMessage(), (m, _) => read(m), null);
// 	}
// }
