using System.Diagnostics;
using Wtq.Core.Configuration;
using Wtq.Core.Data;
using Wtq.Core.Services;

namespace Wtq.Services;

public class WtqApp(
	IWtqProcessService procService,
	IWtqAppToggleService toggler) : IDisposable
{
	private readonly ILogger _log = Log.For<WtqApp>();
	private readonly IWtqProcessService _procService = procService ?? throw new ArgumentNullException(nameof(procService));
	private readonly IWtqAppToggleService _toggler = toggler ?? throw new ArgumentNullException(nameof(toggler));

	public WtqAppOptions App { get; set; }

	public Process? Process { get; set; }

	/// <summary>
	/// Whether an active process is being tracked by this app instance.
	/// </summary>
	public bool IsActive => Process != null;

	public void Dispose()
	{
		if (Process != null)
		{
			var bounds = _procService.GetWindowRect(Process); // TODO: Restore to original position (when we got a hold of the process).
			bounds.Width = 1280;
			bounds.Height = 800;
			bounds.X = 0;
			bounds.Y = 0;

			_log.LogInformation("Restoring process '{Process}' to its original bounds of '{Bounds}'", ProcessDescription, bounds);

			//Process.SetWindowState(WindowShowStyle.Restore);
			_procService.MoveWindow(Process, bounds);
			_procService.SetTaskbarIconVisibility(Process, true);
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

	public int GetTimeMs(ToggleModifiers mods)
	{
		switch (mods)
		{
			case ToggleModifiers.Instant:
				return 0;

			case ToggleModifiers.SwitchingApps:
				return 50;

			case ToggleModifiers.None:
			default:
				return 150;
		}
	}

	public async Task CloseAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		var ms = GetTimeMs(mods);

		_log.LogInformation("Closing app '{App}' in {Time}ms", this, ms);

		await _toggler.ToggleAsync(this, false, ms);
	}

	public async Task OpenAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		var ms = GetTimeMs(mods);

		_log.LogInformation("Opening app '{App}' in {Time}ms", this, ms);

		await _toggler.ToggleAsync(this, true, ms);
	}

	public override string ToString()
	{
		return $"[App:{App}] [ProcessID:{Process?.Id}] {(Process?.ProcessName ?? "<no process>")}";
	}

	/// <summary>
	/// Updates the state of the <see cref="WtqApp"/> object to reflect running processes on the system.
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
			_procService.SetTaskbarIconVisibility(Process, false);

			_log.LogInformation("Found process instance for app '{App}' with name '{ProcessName}' and Id '{ProcessId}'", App, Process.ProcessName, Process.Id);
		}
	}

	public void BringToForeground()
	{
		_procService.BringToForeground(Process);
	}

	public WtqRect GetWindowRect()
	{
		return _procService.GetWindowRect(Process);
	}

	public void MoveWindow(WtqRect rect)
	{
		_procService.MoveWindow(Process, rect: rect);
	}
}