using WindowsTerminalQuake.Utils;

namespace WindowsTerminalQuake.TerminalProcessProviders;

public class GenericProcessProvider : IProcessProvider
{
	private readonly ILogger _log = Log.For<GenericProcessProvider>();
	private readonly App _app;

	public GenericProcessProvider(App app)
	{
		_app = app;
	}

	/// <summary>
	/// Returns the instance of the running Windows Terminal. Creates one if none is running yet.
	/// </summary>
	/// <param name="args">Any command-line arguments to pass to the terminal process if we're starting it.</param>
	public Process Get()
	{
		return Retry.ExecuteAsync(async () =>
		{
			if (_isExitting) return _process!;

			if (_process == null || _process.HasExited)
			{
				_process = GetOrCreate();
			}

			return _process;
		}).GetAwaiter().GetResult();
	}

	private Process? _process;

	private readonly List<Action> _onExit = new();

	private bool _isExitting;

	public void OnExit(Action action)
	{
		_onExit.Add(action);
	}

	private void FireOnExit()
	{
		_isExitting = true;

		_onExit.ForEach(a => a());
	}

	private Process GetOrCreate()
	{
		// Attempt to find an existing process instance first.
		_log.LogInformation("Attempting to get a process for app '{App}'", _app);

		var process = Process
			.GetProcesses()
			.FirstOrDefault(_app.Filter);

		// If we didn't find anything, attempt to create a new process now.
		if (process == null)
		{
			process = _app.NewProcess.Create();

			try
			{
				process.Start();
				process.WaitForInputIdle();
			}
			catch (Win32Exception ex) when (ex.Message == "The system cannot find the file specified")
			{
				_log.LogError("Could not start process for app '{App}', probably due to the targeted file or command not existing", _app);
				throw new CancelRetryException($"Could not find the process exe at '{QSettings.Instance.WindowsTerminalCommand}'. Make sure it is installed, and see '{nameof(QSettings.Instance.WindowsTerminalCommand)}' setting for more information.");
			}

			return process;
		}

		// Try the "nice way" of waiting for the process to become ready.
		process.Refresh();
		process.WaitForInputIdle();

		_log.LogInformation(
			"Got process with id '{ProcessId}' and name '{ProcessName}' and title '{MainWindowTitle}'",
			process.Id,
			process.ProcessName,
			process.MainWindowTitle);

		// Make sure the process has not exited.
		if (process.HasExited)
		{
			throw new WindowsTerminalQuakeException("Process exiting.");
		}

		// Make sure we can access the main window handle.
		// Note: Accessing mainWindowHandle already throws "Process has exited, so the requested information is not available.".
		if (process.MainWindowHandle == IntPtr.Zero)
		{
			throw new WindowsTerminalQuakeException("Main window handle not accessible.");
		}

		process.EnableRaisingEvents = true;
		process.Exited += (s, a) => FireOnExit();

		return process;
	}
}