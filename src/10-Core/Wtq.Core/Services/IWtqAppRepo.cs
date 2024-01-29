using Wtq.Configuration;
using Wtq.Services;

namespace Wtq.Core.Services;

public interface IWtqAppRepo : IDisposable
{
	IReadOnlyCollection<WtqApp> Apps { get; }

	WtqApp? GetProcessForApp(WtqAppOptions app);
}