using Wtq.Data;

namespace Wtq.Services;

public interface IWtqProcessService
{
	void BringToForeground(Process process);

	Process? GetForegroundProcess();

	uint GetForegroundProcessId();

	WtqRect GetWindowRect(Process process);

	void MoveWindow(Process process, WtqRect rect, bool repaint = true);

	void SetAlwaysOnTop(Process process);

	void SetTaskbarIconVisibility(Process process, bool isVisible);

	void SetTransparency(Process process, int transparency);

	IEnumerable<Process> GetProcesses();
}