namespace Wtq.Services.Linux;

public static class XDG
{
	/// <summary>
	/// $XDG_STATE_HOME defines the base directory relative to which user-specific state files should be stored.
	/// If $XDG_STATE_HOME is either not set or empty, a default equal to $HOME/.local/state should be used.
	///
	/// The $XDG_STATE_HOME contains state data that should persist between (application) restarts, but that is
	/// not important or portable enough to the user that it should be stored in $XDG_DATA_HOME.
	///
	/// It may contain:
	/// - actions history (logs, history, recently used files, …)
	/// - current state of the application that can be reused on a restart (view, layout, open files, undo history, …).
	/// </summary>
	public static string XDG_STATE_HOME =>
		EnvUtils.GetEnvVarOrDefault("XDG_STATE_HOME", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local/state"));
}