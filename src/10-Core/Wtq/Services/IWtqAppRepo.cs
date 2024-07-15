namespace Wtq.Services;

public interface IWtqAppRepo : IAsyncDisposable
{
	IEnumerable<WtqApp> Apps { get; }

	WtqApp? GetAppByName(string name);

	WtqApp GetAppByNameRequired(string name);

	Task UpdateAppsAsync();
}