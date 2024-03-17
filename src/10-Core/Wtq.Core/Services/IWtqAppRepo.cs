using Wtq.Core.Configuration;
using Wtq.Services;

namespace Wtq.Core.Services;

public interface IWtqAppRepo : IAsyncDisposable
{
	IReadOnlyCollection<WtqApp> Apps { get; }

	WtqApp? GetProcessForApp(WtqAppOptions app);
}