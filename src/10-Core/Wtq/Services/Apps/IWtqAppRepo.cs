﻿namespace Wtq.Services.Apps;

public interface IWtqAppRepo : IAsyncDisposable
{
	IEnumerable<WtqApp> Apps { get; }

	WtqApp? GetAppByName(string name);

	WtqApp GetAppByNameRequired(string name);

	WtqAppOptions? GetOptionsByName(string name);

	WtqAppOptions GetOptionsByNameRequired(string name);

	Task UpdateAppsAsync();
}