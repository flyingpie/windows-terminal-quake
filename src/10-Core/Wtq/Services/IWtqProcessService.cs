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
	/// Looks for a window that matches the specified <paramref name="opts"/>.
	/// </summary>
	Task<WtqWindow?> FindWindowAsync(WtqAppOptions opts);

	/// <summary>
	/// Returns the window that currently has UI focus.
	/// </summary>
	Task<WtqWindow?> GetForegroundWindowAsync();

	/// <summary>
	/// Returns a list of all windows that can be attached to.<br/>
	/// Includes the ones we may already have attached to.
	/// </summary>
	Task<ICollection<WtqWindow>> GetWindowsAsync();
}