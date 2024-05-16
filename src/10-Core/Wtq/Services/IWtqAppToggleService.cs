namespace Wtq.Services;

/// <summary>
/// Handles moving the windows of the specified <see cref="WtqApp"/> objects on- and off the screen.
/// </summary>
public interface IWtqAppToggleService
{
	/// <summary>
	/// Move a window off the screen.
	/// </summary>
	Task ToggleOffAsync(WtqApp app, ToggleModifiers mods);

	/// <summary>
	/// Move a window onto the screen.
	/// </summary>
	Task ToggleOnAsync(WtqApp app, ToggleModifiers mods);
}