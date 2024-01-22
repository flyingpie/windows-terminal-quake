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
			var bounds = Process.GetBounds(); // TODO: Restore to original position (when we got a hold of the process).
			bounds.Width = 1280;
			bounds.Height = 800;
			bounds.X = 0;
			bounds.Y = 0;

			_log.LogInformation("Restoring process '{Process}' to its original bounds of '{Bounds}'", ProcessDescription, bounds);

			Process.SetWindowState(WindowShowStyle.Restore);
			Process.MoveWindow(bounds);
			Process.SetTaskbarIconVisibility(true);
		}
	}

	public string? ProcessDescription
	{
		get
		{
			if (Process == null)
			{
				return "<no process attached>";
			}

			return $"[{Process?.Id}] {Process?.ProcessName}";
		}
	}

	public void Toggle(bool isVisible)
	{
	}

	public override string ToString()
	{
		return $"[App:{App}] [ProcessID:{Process?.Id}] {(Process?.ProcessName ?? "<no process>")}";
	}

	/// <summary>
	/// Updates the state of the <see cref="WtqProcess"/> object to reflect running processes on the system.
	/// </summary>
	public async Task UpdateAsync(IEnumerable<Process> processes)
	{
		// Check that if we have a process handle, the process is still active.
		if (Process?.HasExited ?? false)
		{
			_log.LogInformation("Process with name '{ProcessName}' and id '{ProcessId}' exited, releasing handle", Process.ProcessName, Process.Id);
			Process = null;
		}

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