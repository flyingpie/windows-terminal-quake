namespace Wtq.Services;

/// <summary>
/// Attempts to fetch <see cref="WtqWindow"/> instances, for an app as configured through <see cref="WtqAppOptions"/>.
/// </summary>
public interface IWtqWindowResolver
{
	/// <summary>
	/// Returns a window handle for the specified app. Can return null, if no window could be found.
	/// </summary>
	/// <param name="opts">The settings associated with the app the find a window for.</param>
	/// <param name="allowStartNew">Whether a new window may be created if necessary (and configured).</param>
	/// <returns>The window handle, if one was found.</returns>
	Task<WtqWindow?> GetWindowHandleAsync(
		WtqAppOptions opts,
		bool allowStartNew);
}