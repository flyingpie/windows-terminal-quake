namespace Wtq.Services;

public interface IWtqUIService
{
	Task CloseMainWindowAsync();

	Task OpenMainWindowAsync();

	void RunOnUIThread(Action action);
}