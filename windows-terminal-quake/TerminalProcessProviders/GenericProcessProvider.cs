using WindowsTerminalQuake.ProcessProviders;

namespace WindowsTerminalQuake.TerminalProcessProviders;

public class GenericProcessProvider : IProcessProvider
{
	/// <summary>
	/// Returns the instance of the running Windows Terminal. Creates one if none is running yet.
	/// </summary>
	/// <param name="args">Any command-line arguments to pass to the terminal process if we're starting it.</param>
	public Process Get(string[] args)
	{
		return Retry.Execute(() =>
		{
			if (_isExitting) return _process!;

			if (_process == null || _process.HasExited)
			{
				_process = GetOrCreate(args);
			}

			return _process;
		});
	}

	private static readonly RetryPolicy Retry = Policy
		.Handle<Exception>()
		.WaitAndRetry(new[]
			{
				TimeSpan.FromMilliseconds(500),
				TimeSpan.FromMilliseconds(1000),
				TimeSpan.FromMilliseconds(1000),
				TimeSpan.FromMilliseconds(1000),
				TimeSpan.FromMilliseconds(1000)
			},
			onRetry: (ex, t) => Log.Error($"Error creating process: '{ex.Message}'"));

	private Process? _process;

	private List<Action> _onExit = new List<Action>();

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

	private Process GetOrCreate(string[] args)
	{
		const string existingProcessName = "wezterm-gui";

		QSettings.Instance.WindowsTerminalCommand = @"D:\syncthing\apps\wezterm\wezterm-gui.exe";

		var process = Process.GetProcessesByName(existingProcessName).FirstOrDefault();
		if (process == null)
		{
			process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = QSettings.Instance.WindowsTerminalCommand,
					Arguments = string.Join(" ", args),
					UseShellExecute = false
				}
			};

			try
			{
				process.Start();
				process.WaitForInputIdle();
			}
			catch (Win32Exception ex) when (ex.Message == "The system cannot find the file specified")
			{
				throw new WindowsTerminalQuakeException($"Could not find the process exe at '{QSettings.Instance.WindowsTerminalCommand}'. Make sure it is installed, and see '{nameof(QSettings.Instance.WindowsTerminalCommand)}' setting for more information.");
			}

			// After starting the process, just throw an exception so the process search gets restarted.
			// The "wt.exe" process does some stuff to ultimately fire up a "WindowsTerminal" process, so we can't actually use the Process instance we just created.
			throw new WindowsTerminalQuakeException("Started process");
		}

		// Try the "nice way" of waiting for the process to become ready
		process.Refresh();
		process.WaitForInputIdle();

		Log.Information(
			$"Got process with id '{process.Id}' and name '{process.ProcessName}' and title '{process.MainWindowTitle}'.");

		// Make sure the process has not exited
		if (process.HasExited) throw new WindowsTerminalQuakeException($"Process existing.");

		// Make sure we can access the main window handle
		// Note: Accessing mainWindowHandle already throws "Process has exited, so the requested information is not available."
		if (process.MainWindowHandle == IntPtr.Zero) throw new WindowsTerminalQuakeException("Main window handle not accessible.");

		process.EnableRaisingEvents = true;
		process.Exited += (s, a) => FireOnExit();

		return process;
	}
}