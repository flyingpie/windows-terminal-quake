namespace Wtq;

public static class WtqConstants
{
	public static string AppVersion { get; }
		= typeof(WtqApp).Assembly.GetName().Version?.ToString() ?? "<unknown>";

	public static Uri GitHubUrl { get; }
		= new("https://www.github.com/flyingpie/windows-terminal-quake");
}