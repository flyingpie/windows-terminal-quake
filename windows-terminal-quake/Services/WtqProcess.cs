using System.Diagnostics;
using Wtq.Configuration;
using Wtq.Native;
using Wtq.Native.Win32;

namespace Wtq.Services;

public class WtqProcess : IDisposable
{
	private readonly ILogger _log = Log.For<WtqProcess>();

	public WtqAppOptions App { get; set; }

	public Process? Process { get; set; }

	public void Dispose()
	{
		if (Process != null)
		{
			Process.SetTaskbarIconVisibility(true);
			Process.SetWindowState(WindowShowStyle.Maximize);
		}
	}

	public void Toggle(bool isVisible)
	{
	}

	public override string ToString()
	{
		return $"[{App}] {(Process?.ProcessName ?? "<no process>")}";
	}

	/// <summary>
	/// Updates the state of the <see cref="WtqProcess"/> object to reflect running processes on the system.
	/// </summary>
	public async Task UpdateAsync(IEnumerable<Process> processes)
	{
		if (Process == null)
		{
			// TODO: Handle multiple processes coming back?
			Process = processes
				.FirstOrDefault(App.FindExisting.Filter);

			if (Process == null)
			{
				_log.LogWarning("No process instances found for app '{App}'", App);
				// TODO: Create process.
				return;
			}

			// TODO: Configurable.
			Process.SetTaskbarIconVisibility(false);

			_log.LogInformation("Found process instance for app '{App}' with name '{ProcessName}' and Id '{ProcessId}'", App, Process.ProcessName, Process.Id);
		}
	}
}