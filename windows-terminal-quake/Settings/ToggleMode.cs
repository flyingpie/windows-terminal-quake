namespace WindowsTerminalQuake.Settings;

public enum ToggleMode
{
	None = 0,

	/// <summary>
	/// Use the initial terminal position and size to toggle to- and from. Does not resize or reposition.
	/// </summary>
	Initial,

	/// <summary>
	/// "Move" keeps the size of the terminal constant, but moves the terminal off-screen to the top, which won't work great with vertically stacked monitors.
	/// </summary>
	Move,

	/// <summary>
	/// "Resize" should work on any setup, but may cause characters in the terminal to jump around after toggling.
	/// </summary>
	Resize
}