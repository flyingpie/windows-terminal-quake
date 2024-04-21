namespace Wtq.Core.Configuration;

public enum TaskBarIconVisibility
{
	/// <summary>
	/// Never show the task bar icon.
	/// </summary>
	AlwaysHidden,

	/// <summary>
	/// Always show the task bar icon (note that this can look a bit weird when the app is toggled off).
	/// </summary>
	AlwaysVisible,

	/// <summary>
	/// Only show the task bar icon when the app is toggled on.
	/// </summary>
	WhenAppVisible,
}