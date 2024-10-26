namespace Wtq.Services;

/// <summary>
/// The app repo keeps track of "apps", as configured in the settings file. An app is mostly an entity
/// that describes a window that can be toggled, and how and when to do that.
/// </summary>
public interface IWtqAppRepo : IAsyncDisposable
{
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
	/// Returns the p
	/// </summary>
	/// <returns></returns>
	// WtqApp? GetOpen();

	/// <summary>
	/// Returns the "primary" app, currently just the first one in the settings list (if any).
	/// </summary>
	WtqApp? GetPrimary();
}