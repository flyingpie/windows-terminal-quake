namespace Wtq.Services;

/// <summary>
/// Handles interactions with application windows.
/// </summary>
public interface IWtqWindowService
{
	/// <summary>
	/// Start an application, so we can attach to its window later. The specified <see cref="WtqAppOptions"/> defines startup parameters.
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