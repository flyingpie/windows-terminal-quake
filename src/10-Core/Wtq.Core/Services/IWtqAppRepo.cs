namespace Wtq.Core.Services;

public interface IWtqAppRepo : IAsyncDisposable
{
	IReadOnlyCollection<WtqApp> Apps { get; }

	Task UpdateAppsAsync();

	WtqApp? GetAppByName(string name);

	WtqApp GetAppByNameRequired(string name);

	WtqAppOptions? GetOptionsByName(string name);

	WtqAppOptions GetOptionsByNameRequired(string name);
}