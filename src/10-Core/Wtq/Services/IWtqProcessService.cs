namespace Wtq.Services;

/// <summary>
/// Handles interactions with the OS process list.
/// </summary>
public interface IWtqProcessService
{
	/// <summary>
	/// Create a new process instance as defined by <see cref="WtqAppOptions"/>'s parameters.
	/// </summary>
	Task CreateAsync(WtqAppOptions opts);

	/// <summary>
	/// Looks for a process that matches the specified <paramref name="opts"/>.
	/// </summary>
	Task<WtqWindow?> FindProcess(WtqAppOptions opts);

	/// <summary>
	/// Returns the process that currently has UI focus.
	/// </summary>
	WtqWindow? GetForegroundWindow();
}