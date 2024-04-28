namespace Wtq;

public static class AppPaths
{
	private static string? _pathToAppConf;
	private static string? _pathToAppDir;
	private static string? _pathToAppExe;

	public static string PathToAppConf
	{
		get
		{
			return _pathToAppConf ??= Path.Combine(PathToAppDir, "wtq.jsonc");
		}
	}

	[SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations", Justification = "MvdO: Yeah, not great.")]
	public static string PathToAppDir
	{
		get
		{
			return _pathToAppDir ??= Path.GetDirectoryName(PathToAppExe)
				?? throw new WtqException($"Could not get path to app directory (path to app executable: '{PathToAppExe}').");
		}
	}

	[SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations", Justification = "MvdO: Yeah, not great.")]
	public static string PathToAppExe
	{
		get
		{
			return _pathToAppExe ??= Environment.ProcessPath
				?? throw new WtqException("Could not find path to wtq exe.");
		}
	}
}