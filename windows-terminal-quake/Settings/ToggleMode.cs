namespace WindowsTerminalQuake.Settings
{
	public enum ToggleMode
	{
		/// <summary>
		/// "Move" keeps the size of the terminal constant, but moves the terminal off-screen to the top, which won't work great with vertically stacked monitors.
		/// </summary>
		Move,

		/// <summary>
		/// "Resize" should work on any setup, but may cause characters in the terminal to jump around after toggling.
		/// </summary>
		Resize
	}
}