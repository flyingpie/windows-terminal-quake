namespace Wtq.Services;

/// <summary>
/// The app repo keeps track of <see cref="WtqApp"/> instances, as configured in the settings file.
/// </summary>
public interface IWtqAppRepo
{
	/// <summary>
	/// Returns all apps as loaded from settings.
	/// </summary>
	IEnumerable<WtqApp> GetAll();

	/// <summary>
	/// Returns the <see cref="WtqApp"/> instance for the <paramref name="name"/>, as specified in the app config (if any).
	/// </summary>
	WtqApp? GetByName(
		string name);

	/// <summary>
	/// Returns the <see cref="WtqApp"/> instance for the specified <paramref name="window"/> (if any).
	/// </summary>
	WtqApp? GetByWindow(
		WtqWindow window);

	/// <summary>
	/// Returns the app that is currently toggled onto the screen (if any).
	/// </summary>
	WtqApp? GetOpen();

	/// <summary>
	/// Returns the "primary" app, currently just the first one in the settings list (if any).
	/// </summary>
	WtqApp? GetPrimary();
}