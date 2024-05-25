using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin;

internal class KWinScriptExecutor
{
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

		var scriptId = Guid.NewGuid().ToString();
		var path = $"/dev/shm/wtq-{scriptId}.js";

		try
		{
			await File.WriteAllTextAsync(path, script, cancellationToken).ConfigureAwait(false);

			await _scripting.LoadScriptAsync(path, scriptId).ConfigureAwait(false);
			await _scripting.StartAsync().ConfigureAwait(false);
			await _scripting.UnloadScriptAsync(scriptId).ConfigureAwait(false);
		}
		finally
		{
			File.Delete(path);
		}
	}

	public async Task<TResult> ExecuteAsync<TResult>(
		Guid id,
		string script,
		CancellationToken cancellationToken)
	{
		Guard.Against.NullOrWhiteSpace(script);

		var scriptId = id.ToString();
		var path = $"/dev/shm/wtq-{id}.js";

		var dbus = (WtqDBusObject)await _wtqDBusObj.ConfigureAwait(false);
		var waiter = dbus.CreateResponseWaiter(id);

		try
		{
			await File.WriteAllTextAsync(path, script, cancellationToken).ConfigureAwait(false);

			await _scripting.LoadScriptAsync(path, scriptId).ConfigureAwait(false);
			await _scripting.StartAsync().ConfigureAwait(false);
			await _scripting.UnloadScriptAsync(scriptId).ConfigureAwait(false);

			return await waiter.GetResultAsync<TResult>(cancellationToken).ConfigureAwait(false);
		}
		finally
		{
			File.Delete(path);
		}
	}
}