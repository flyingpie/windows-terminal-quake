using Wtq.Core.Configuration;

namespace Wtq.Services;

public interface IWtqAppFactory
{
	WtqApp Create(WtqAppOptions app);
}