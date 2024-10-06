using Wtq.Services.KWin.DBus;

public interface IKWinScriptService
{
	Task<bool> IsScriptLoadedAsync(string scriptId);

	Task LoadScriptAsync(string path, string scriptId);

	Task StartAsync();

	Task UnloadScriptAsync(string scriptId);
}

internal class KWinScriptService(
	Scripting scripting) : IKWinScriptService
{
	private readonly Scripting _scripting = Guard.Against.Null(scripting);

	public Task<bool> IsScriptLoadedAsync(string scriptId)
	{
		return _scripting.IsScriptLoadedAsync(scriptId);
	}

	public Task LoadScriptAsync(string path, string scriptId)
	{
		return _scripting.LoadScriptAsync(path, scriptId);
	}

	public Task StartAsync()
	{
		return _scripting.StartAsync();
	}

	public Task UnloadScriptAsync(string scriptId)
	{
		return _scripting.UnloadScriptAsync(scriptId);
	}
}