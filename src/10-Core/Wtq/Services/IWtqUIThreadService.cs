namespace Wtq.Services;

public interface IWtqUIThreadService
{
	void OpenMainWindow();

	void RunOnUIThread(Action action);
}