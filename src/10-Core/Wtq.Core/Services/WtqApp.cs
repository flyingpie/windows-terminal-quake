using Wtq.Core.Configuration;
using Wtq.Core.Data;
using Wtq.Core.Services;

namespace Wtq.Services;

/// <summary>
/// An "app" represents a single process that can be toggled (such as Windows Terminal).<br/>
/// It tracks its own state, and does not necessarily have a process attached.
/// </summary>
public sealed class WtqApp : IAsyncDisposable
{
	private readonly ILogger _log = Log.For<WtqApp>();
	private readonly IWtqProcessFactory _procFactory;
	private readonly IWtqProcessService _procService;
	private readonly IWtqAppToggleService _toggler;

	public WtqApp(
		IWtqProcessFactory procFactory,
		IWtqProcessService procService,
		IWtqAppToggleService toggler,
		IWtqBus bus,
		WtqAppOptions opts)
	{
		_procFactory = procFactory ?? throw new ArgumentNullException(nameof(procFactory));
		_procService = procService ?? throw new ArgumentNullException(nameof(procService));
		_toggler = toggler ?? throw new ArgumentNullException(nameof(toggler));

		App = opts ?? throw new ArgumentNullException(nameof(opts));
	}

	public WtqAppOptions App { get; }

	/// <summary>
	/// Whether an active process is being tracked by this app instance.
	/// </summary>
	public bool IsActive => Process != null;

	public Process? Process { get; set; }

	public string? ProcessDescription => Process == null
		? "<no process attached>"
		: $"[{Process.Id}] {Process.ProcessName}";

	// TODO: Pull from options.
	private static int GetTimeMs(ToggleModifiers mods)
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

	/// <summary>
	/// Puts the window associated with the process on top of everything and gives it focus.
	/// </summary>
	public void BringToForeground()
	{
		if (Process == null)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		_procService.BringToForeground(Process);
	}

	public async Task CloseAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		var ms = GetTimeMs(mods);

		_log.LogInformation("Closing app '{App}' in {Time}ms", this, ms);

		await _toggler.ToggleAsync(this, false, ms).ConfigureAwait(false);
	}

	public async ValueTask DisposeAsync()
	{
		// TODO: Add ability to close attached processes when app closes.
		if (Process != null)
		{
			var bounds = _procService.GetWindowRect(Process); // TODO: Restore to original position (when we got a hold of the process).
			bounds.Width = 1280;
			bounds.Height = 800;
			bounds.X = 10;
			bounds.Y = 10;

			_log.LogInformation("Restoring process '{Process}' to its original bounds of '{Bounds}'", ProcessDescription, bounds);

			//_procService.MoveWindow(Process, bounds);

			await OpenAsync(ToggleModifiers.Instant).ConfigureAwait(false);

			_procService.SetTaskbarIconVisibility(Process, true);
		}
	}

	public WtqRect? GetWindowRect()
	{
		if (Process == null)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		return _procService.GetWindowRect(Process);
	}

	public void MoveWindow(WtqRect rect)
	{
		if (Process == null)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		_procService.MoveWindow(Process, rect: rect);
	}

	public async Task OpenAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		// If we have an active process attached, toggle it open.
		if (IsActive)
		{
			var ms = GetTimeMs(mods);

			_log.LogInformation("Opening app '{App}' in {Time}ms", this, ms);

			await _toggler.ToggleAsync(this, true, ms).ConfigureAwait(false);
		}

		if (!IsActive && App.AttachMode == AttachMode.Manual)
		{
			var pr = _procService.GetForegroundProcess();
			if (pr != null)
			{
				await AttachAsync(pr).ConfigureAwait(false);
			}

			_log.LogWarning("ATTACH?!");
		}
	}

	public override string ToString()
	{
		try
		{
			// TODO: Make extensions to safely pull process info without crashing.
			return $"[App:{App}] [ProcessID:{Process?.Id}] {Process?.ProcessName ?? "<no process>"}";
		}
		catch (Exception ex)
		{
			return $"[App:{App}] <no process>";
		}
	}

	/// <summary>
	/// Updates the state of the <see cref="WtqApp"/> object to reflect running processes on the system.
	/// </summary>
	public async Task UpdateAsync()
	{
		// Check that if we have a process handle, the process is still active.
		if (Process?.HasExited ?? false)
		{
			_log.LogInformation("Process with name '{ProcessName}' and id '{ProcessId}' exited, releasing handle", Process.ProcessName, Process.Id);
			Process = null;
		}

		if (Process == null)
		{
			var process = await _procFactory.GetProcessAsync(App).ConfigureAwait(false);

			if (process == null)
			{
				_log.LogWarning("No process instances found for app '{App}'", App);
				return;
			}

			await AttachAsync(process).ConfigureAwait(false);
		}
	}

	public async Task AttachAsync(Process process)
	{
		Process = process;

		// TODO: Configurable.
		_procService.SetTaskbarIconVisibility(process, false);

		await CloseAsync(ToggleModifiers.Instant).ConfigureAwait(false);

		_log.LogInformation("Found process instance for app '{App}' with name '{ProcessName}' and Id '{ProcessId}'", App, process.ProcessName, process.Id);
	}
}