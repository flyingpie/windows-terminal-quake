//namespace WindowsTerminalQuake.TerminalProcessProviders;

///// <summary>
///// Wrapper around the Windows Terminal process. Contains stuff to actually capture the process,
///// which turns out to be tricky in some cases.
///// </summary>
//public class WindowsTerminalProcessProvider : IProcessProvider
//{
//	private static readonly RetryPolicy Retry = Policy
//		.Handle<Exception>()
//		.WaitAndRetry(
//		new[]
//		{
//			TimeSpan.FromMilliseconds(500),
//			TimeSpan.FromMilliseconds(1000),
//			TimeSpan.FromMilliseconds(1000),
//			TimeSpan.FromMilliseconds(3000),
//			TimeSpan.FromMilliseconds(3000),
//			TimeSpan.FromMilliseconds(3000),
//			TimeSpan.FromMilliseconds(3000),
//			TimeSpan.FromMilliseconds(5000),
//		},
//		onRetry: (ex, t) => Log.Error($"Error creating process: '{ex.Message}'"));

//	private readonly List<Action> _onExit = new();

//	private bool _isExitting;
//	private Process? _process;

//	public void Close()
//	{
//		try
//		{
//			_process?.CloseMainWindow();
//		}
//		catch (Exception ex)
//		{
//			Log.Warning(ex, $"Error closing Windows Terminal process: {ex.Message}");
//		}
//	}

//	/// <summary>
//	/// Returns the instance of the running Windows Terminal. Creates one if none is running yet.
//	/// </summary>
//	/// <param name="args">Any command-line arguments to pass to the terminal process if we're starting it.</param>
//	public Process Get(string[] args)
//	{
//		return Retry.Execute(() =>
//		{
//			if (_isExitting) return _process!;

//			if (_process == null || _process.HasExited)
//			{
//				_process = GetOrCreate(args);
//			}

//			return _process;
//		});
//	}

//	public void OnExit(Action action)
//	{
//		_onExit.Add(action);
//	}

//	private void FireOnExit()
//	{
//		_isExitting = true;

//		_onExit.ForEach(a => a());
//	}

//	private Process GetOrCreate(string[] args)
//	{
//		const string existingProcessName = "WindowsTerminal";

//		var process = Process.GetProcessesByName(existingProcessName).FirstOrDefault();
//		if (process == null)
//		{
//			process = new Process
//			{
//				StartInfo = new ProcessStartInfo
//				{
//					FileName = QSettings.Instance.WindowsTerminalCommand,
//					Arguments = string.Join(" ", args),
//					UseShellExecute = false
//				}
//			};

//			try
//			{
//				process.Start();
//				process.WaitForInputIdle();
//			}
//			catch (Win32Exception ex) when (ex.Message == "The system cannot find the file specified")
//			{
//				throw new WindowsTerminalQuakeException($"Could not find the Windows Terminal exe at '{QSettings.Instance.WindowsTerminalCommand}'. Make sure it is installed, and see '{nameof(QSettings.Instance.WindowsTerminalCommand)}' setting for more information.");
//			}

//			// After starting the process, just throw an exception so the process search gets restarted.
//			// The "wt.exe" process does some stuff to ultimately fire up a "WindowsTerminal" process, so we can't actually use the Process instance we just created.
//			throw new WindowsTerminalQuakeException($"Started process");
//		}

//		// Try the "nice way" of waiting for the process to become ready
//		process.Refresh();
//		process.WaitForInputIdle();

//		Log.Information(
//			$"Got process with id '{process.Id}' and name '{process.ProcessName}' and title '{process.MainWindowTitle}'.");

//		// Make sure the process has not exited
//		if (process.HasExited) throw new WindowsTerminalQuakeException($"Process existing.");

//		// Make sure we can access the main window handle
//		// Note: Accessing mainWindowHandle already throws "Process has exited, so the requested information is not available."
//		if (process.MainWindowHandle == IntPtr.Zero) throw new WindowsTerminalQuakeException("Main window handle not accessible.");

//		// Make sure the process name equals "WindowsTerminal", otherwise WT might still be starting
//		if (process.ProcessName != "WindowsTerminal") throw new WindowsTerminalQuakeException("Process name is not 'WindowsTerminal' yet.");

//		// We need a proper window title before we can continue
//		if (process.MainWindowTitle == "")
//			throw new WindowsTerminalQuakeException($"Process still has temporary '' (empty) window title.");

//		// This is a way-too-specific check to further ensure the WT process is ready
//		if (process.MainWindowTitle == "DesktopWindowXamlSource")
//			throw new WindowsTerminalQuakeException($"Process still has temporary 'DesktopWindowXamlSource' window title.");

//		process.EnableRaisingEvents = true;
//		process.Exited += (s, a) => FireOnExit();

//		return process;
//	}
//}