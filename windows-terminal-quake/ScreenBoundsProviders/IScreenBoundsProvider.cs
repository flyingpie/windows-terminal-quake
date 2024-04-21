namespace WindowsTerminalQuake.ScreenBoundsProviders;

public interface IScreenBoundsProvider
{
	/// <summary>
	/// Returns a bounding box for the screen where the terminal should be positioned on.
	/// </summary>
	Rectangle GetTargetScreenBounds();
}