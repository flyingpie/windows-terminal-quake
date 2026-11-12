namespace Wtq.Host.Linux.E2ETest;

/// <summary>
/// Runs the actual "wtq" executable as a child process, with an isolated settings file and log directory.
/// </summary>
public sealed class WtqProcess : IAsyncDisposable
{
	private readonly Process _process;

	private WtqProcess(Process process, string workDir)
	{
		_process = process;
		WorkDir = workDir;
	}

	/// <summary>
	/// Directory containing the settings file and WTQ's logs for this run.
	/// </summary>
	public string WorkDir { get; }

	public bool HasExited => _process.HasExited;

	/// <summary>
	/// Path to the "wtq" executable. Defaults to the build output of the Wtq.Host.Linux project,
	/// can be overridden using the "WTQ_E2E_EXE" environment variable.
	/// </summary>
	public static string PathToExe =>
		Environment.GetEnvironmentVariable("WTQ_E2E_EXE") is { Length: > 0 } exe
			? exe
			: Path.GetFullPath(
				Path.Combine(
					AppContext.BaseDirectory,
					"..",
					"..",
					"Wtq.Host.Linux",
					"net10.0",
					"wtq"
				)
			);

	public static WtqProcess Start(string wtqJsonc)
	{
		var workDir = Path.Combine(Path.GetTempPath(), $"wtq-e2e-{Guid.NewGuid():N}");
		Directory.CreateDirectory(workDir);

		var pathToConfig = Path.Combine(workDir, "wtq.jsonc");
		File.WriteAllText(pathToConfig, wtqJsonc);

		var startInfo = new ProcessStartInfo(PathToExe)
		{
			WorkingDirectory = workDir,
			UseShellExecute = false,
		};

		// Point WTQ to our settings file.
		startInfo.Environment["WTQ_CONFIG_FILE"] = pathToConfig;

		// Keep logs (and other state) of this run separate from the user's.
		startInfo.Environment["XDG_STATE_HOME"] = workDir;
		startInfo.Environment["WTQ_LOG_LEVEL"] = "Debug";

		var process =
			Process.Start(startInfo)
			?? throw new InvalidOperationException($"Could not start '{PathToExe}'.");

		return new WtqProcess(process, workDir);
	}

	/// <summary>
	/// Stops WTQ gracefully (SIGTERM) so it can clean up its KWin script and shortcuts, falling back to a hard kill.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		if (!_process.HasExited)
		{
			using (
				var kill = Process.Start(
					"kill",
					["-TERM", _process.Id.ToString(CultureInfo.InvariantCulture)]
				)
			)
			{
				await kill.WaitForExitAsync();
			}

			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
			try
			{
				await _process.WaitForExitAsync(cts.Token);
			}
			catch (OperationCanceledException)
			{
				_process.Kill(entireProcessTree: true);
			}
		}

		_process.Dispose();
	}
}
