#pragma warning disable // PoC

using Wtq.Configuration;
using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin;

/// <summary>
/// TODO(MvdO): Here be dragons, this is the messiest part of the proof of concept.
/// </summary>
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
		catch (Exception ex)
		{
			throw;
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

		var scriptId = name;
		var unloaded = await _scripting.UnloadScriptAsync(scriptId).NoCtx();
		var xx2 = await _scripting.IsScriptLoadedAsync(scriptId).NoCtx();

		var kwinMod = "Ctrl";
		var kwinKey = key switch
		{
			Keys.D1 => "1",
			Keys.D2 => "2",
			Keys.D3 => "3",
			Keys.D4 => "4",
			Keys.D5 => "5",
			Keys.D6 => "6",
			Keys.Q => "q",
			_ => "1"
		};

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

		try
		{
			await File.WriteAllTextAsync(path, script, cancellationToken).NoCtx();

			_log.LogInformation("Loading script '{ScriptId}'", scriptId);
			await _scripting.LoadScriptAsync(path, scriptId).NoCtx();
			await _scripting.StartAsync().NoCtx();

			_log.LogInformation("Loaded script '{ScriptId}'", scriptId);
		}
		finally
		{
			_log.LogInformation("Executed script in {ElapsedMs}ms", sw.ElapsedMilliseconds);
		}
	}
}