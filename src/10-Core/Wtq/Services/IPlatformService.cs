namespace Wtq.Services;

public interface IPlatformService
{
	/// <summary>
	/// A short name for the current platform. Used in the tray icon context menu and the GUI.<br/>
	/// E.g. "Flatpak", "Windows", "Linux".
	/// </summary>
	string PlatformName { get; }

	/// <summary>
	/// When no addresses are specified to run the API on, these used.
	/// </summary>
	ICollection<string> DefaultApiUrls { get; }

	/// <summary>
	/// Path to the directory that contains the WTQ app itself.
	/// </summary>
	string PathToAppDir { get; }

	/// <summary>
	/// Path to the icon that's used by the GUI (including in the taskbar, but not the tray icon).
	/// </summary>
	string PathToAppIcon { get; }

	string PathToAssetsDir { get; }

	/// <summary>
	/// Where the log files are stored.
	/// </summary>
	string PathToLogsDir { get; }

	/// <summary>
	///
	/// </summary>
	string PathToTempDir { get; }

	/// <summary>
	/// Path to the icon that's used for the tray icon.
	/// </summary>
	string PathToTrayIcon { get; }

	/// <summary>
	/// Path to the user's home directory. Used to resolve settings.
	/// </summary>
	string PathToUserHomeDir { get; }

	/// <summary>
	/// Path to the active (usually only) settings file.
	/// </summary>
	string PathToWtqConf { get; }

	/// <summary>
	/// Path to the active (usually only) settings file's directory.
	/// </summary>
	string PathToWtqConfDir { get; }

	/// <summary>
	/// Ordered list of paths where the settings file could be.
	/// </summary>
	ICollection<string> PathsToWtqConfs { get; }

	/// <summary>
	/// If no settings file is found, this property is used to create one.<br/>
	/// Note that this is not necessarily the first path in <see cref="PathsToWtqConfs"/>, hence why it's a separate property.
	/// </summary>
	string PreferredPathWtqConfig { get; }

	Process CreateProcess(WtqAppOptions opts);

	/// <summary>
	/// Returns whether a <paramref name="fileName"/> can be called, optionally using the specified <paramref name="workingDirectory"/>.<br/>
	/// Used to tell the user whether a file name looks correct, in GUI (for examples and apps).
	/// </summary>
	bool IsCallable(string? workingDirectory, string fileName);

	/// <summary>
	/// Opens the specified <paramref name="path"/> using the default application (e.g. usually Explorer on Windows).
	/// </summary>
	void OpenFileOrDirectory(string path);

	/// <summary>
	/// Opens the specified <paramref name="url"/> using the default application (usually a web browser).
	/// </summary>
	void OpenUrl(Uri url);

	string? ResolvePath(string fileName);

	/// <summary>
	/// Returns whether - when watching for changes for the specified <paramref name="path"/> - a polling file watcher should be used.
	/// </summary>
	bool ShouldUsePollingFileWatcherForPath(string path);
}