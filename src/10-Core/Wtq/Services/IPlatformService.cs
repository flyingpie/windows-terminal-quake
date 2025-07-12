namespace Wtq.Services;

public interface IPlatformService
{
	/// <summary>
	/// A short name for the current platform. Used in the tray icon context menu and the GUI.<br/>
	/// E.g. "Flatpak", "Windows", "Linux".
	/// </summary>
	string PlatformName { get; }

	/// <summary>
	/// Where the log files are stored.
	/// </summary>
	string PathToLogs { get; }

	/// <summary>
	/// Path to the active (usually only) settings file.
	/// </summary>
	string PathToWtqConf { get; set; }

	/// <summary>
	/// Ordered list of paths where the settings file could be.
	/// </summary>
	ICollection<string> PathsToWtqConfs { get; }

	/// <summary>
	/// If no settings file is found, this property is used to create one.<br/>
	/// Note that this is not necessarily the first path in <see cref="PathsToWtqConfs"/>, hence why it's a separate property.
	/// </summary>
	string PreferredPathWtqConfig { get; }

	/// <summary>
	/// Returns whether a <paramref name="fileName"/> can be called, optionally using the specified <paramref name="workingDirectory"/>.<br/>
	/// Used to tell the user whether a file name looks correct, in GUI (for examples and apps).
	/// </summary>
	bool IsCallable(string? workingDirectory, string fileName);

	/// <summary>
	/// Returns whether - when watching for changes for the specified <paramref name="path"/> - a polling file watcher should be used.
	/// </summary>
	bool ShouldUsePollingFileWatcherForPath(string path);

	/// <summary>
	/// Opens the specified <paramref name="path"/> using the default application (e.g. usually Explorer on Windows).
	/// </summary>
	void OpenFileOrDirectory(string path);

	/// <summary>
	/// Opens the specified <paramref name="url"/> using the default application (usually a web browser).
	/// </summary>
	void OpenUrl(Uri url);
}