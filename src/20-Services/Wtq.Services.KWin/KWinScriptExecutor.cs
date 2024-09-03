using Wtq.Configuration;
using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin;

internal class KWinScriptExecutor
{
	private readonly ILogger _log = Log.For<KWinScriptExecutor>();
	private readonly Task<IWtqDBusObject> _wtqDBusObj;
	private readonly Scripting _scripting;

	public KWinScriptExecutor(
		Task<IWtqDBusObject> wtqDBusObj,
		Scripting scripting)
	{
		_wtqDBusObj = Guard.Against.Null(wtqDBusObj);
		_scripting = Guard.Against.Null(scripting);
	}

	public async Task ExecuteAsync(
		string script,
		CancellationToken cancellationToken)
	{
		Guard.Against.NullOrWhiteSpace(script);

		var sw = Stopwatch.StartNew();

		var scriptId = Guid.NewGuid().ToString();
		var path = $"/dev/shm/wtq-{scriptId}.js";

		try
		{
			await File.WriteAllTextAsync(path, script, cancellationToken).NoCtx();

			await _scripting.LoadScriptAsync(path, scriptId).NoCtx();
			await _scripting.StartAsync().NoCtx();
			await _scripting.UnloadScriptAsync(scriptId).NoCtx();
		}
		finally
		{
			File.Delete(path);

			_log.LogInformation("Executed script in {ElapsedMs}ms", sw.ElapsedMilliseconds);
		}
	}

	public async Task<TResult> ExecuteAsync<TResult>(
		Guid id,
		string script,
		CancellationToken cancellationToken)
	{
		Guard.Against.NullOrWhiteSpace(script);

		var sw = Stopwatch.StartNew();

		var scriptId = id.ToString();
		var path = $"/dev/shm/wtq-{id}.js";

		var dbus = (WtqDBusObject)await _wtqDBusObj.NoCtx();
		var waiter = dbus.CreateResponseWaiter(id);

		try
		{
			await File.WriteAllTextAsync(path, script, cancellationToken).NoCtx();

			await _scripting.LoadScriptAsync(path, scriptId).NoCtx();
			await _scripting.StartAsync().NoCtx();
			await _scripting.UnloadScriptAsync(scriptId).NoCtx();

			return await waiter.GetResultAsync<TResult>(cancellationToken).NoCtx();
		}
		finally
		{
			File.Delete(path);

			_log.LogInformation("Executed script in {ElapsedMs}ms", sw.ElapsedMilliseconds);
		}
	}

	public async Task RegisterHotkeyAsync(string name, KeyModifiers mod, Keys key)
	{
		var cancellationToken = CancellationToken.None;

		_log.LogInformation("Registering hotkey");

		var sw = Stopwatch.StartNew();

		// for (int i = 0; i < 50; i++)
		// {
		// 	var sc = $"wtq_{i:000}";
		// 	var res = await _scripting.UnloadScriptAsync(sc);
		// 	_log.LogInformation("Unloaded script {Script}: {Res}", sc, res);
		// }

		//		var scriptId = "wtq-hk-001";
		var scriptId = name;
		var unloaded = await _scripting.UnloadScriptAsync(scriptId);
		var xx2 = await _scripting.IsScriptLoadedAsync(scriptId);

		var kwinMod = "Ctrl";
		var kwinKey = "1";
		if (key == Keys.D1)
		{
			kwinKey = "1";
		}

		if (key == Keys.D2)
		{
			kwinKey = "2";
		}

		if (key == Keys.D3)
		{
			kwinKey = "3";
		}

		if (key == Keys.D4)
		{
			kwinKey = "4";
		}

		if (key == Keys.Q)
		{
			kwinKey = "q";
		}

		var kwinSequence = $"{kwinMod}+{kwinKey}";

		var path = $"/dev/shm/{scriptId}.js";
		var script = $$"""
			console.log("Registering shortcut");
			registerShortcut(
				"{{name}}_text",
				"{{name}}_title",
				"{{kwinSequence}}",
				() => {
					console.log("BLEH! Fire shortcut '{{kwinSequence}}'");
					callDBus("wtq.svc", "/wtq/kwin", "wtq.kwin", "OnPressShortcut", "{{mod}}", "{{key}}");
					console.log("BLEH! /Fire shortcut '{{kwinSequence}}'");
				}
			);
			console.log("/Registering shortcut");
			""";

		// var dbus = (WtqDBusObject)await _wtqDBusObj.NoCtx();
		// var waiter = dbus.CreateResponseWaiter(id);

		try
		{
			await File.WriteAllTextAsync(path, script, cancellationToken).NoCtx();

			_log.LogInformation("Loading script '{ScriptId}'", scriptId);
			await _scripting.LoadScriptAsync(path, scriptId).NoCtx();
			await _scripting.StartAsync().NoCtx();

			// await _scripting.UnloadScriptAsync(scriptId).NoCtx();

			// return await waiter.GetResultAsync<TResult>(cancellationToken).NoCtx();

			_log.LogInformation("Loaded script '{ScriptId}'", scriptId);
		}
		finally
		{
			// File.Delete(path);

			_log.LogInformation("Executed script in {ElapsedMs}ms", sw.ElapsedMilliseconds);
		}
	}
}