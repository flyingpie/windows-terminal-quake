using Wtq.Data;
using Wtq.Services;
using Wtq.Services.Apps;

namespace Wtq;

/// <summary>
/// An "app" represents a single process that can be toggled (such as Windows Terminal).<br/>
/// It tracks its own state, and does not necessarily have a process attached.
/// </summary>
public sealed class WtqApp : IAsyncDisposable
{
	private readonly ILogger _log = Log.For<WtqApp>();

	private readonly Func<WtqAppOptions> _optionsAccessor;
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqProcessFactory _procFactory;
	private readonly IWtqProcessService _procService;
	private readonly IWtqAppToggleService _toggler;

	public WtqApp(
		IOptionsMonitor<WtqOptions> opts,
		IWtqProcessFactory procFactory,
		IWtqProcessService procService,
		IWtqAppToggleService toggler,
		Func<WtqAppOptions> optionsAccessor,
		string name)
	{
		_opts = Guard.Against.Null(opts, nameof(opts));
		_procFactory = procFactory ?? throw new ArgumentNullException(nameof(procFactory));
		_procService = procService ?? throw new ArgumentNullException(nameof(procService));
		_toggler = toggler ?? throw new ArgumentNullException(nameof(toggler));
		_optionsAccessor = Guard.Against.Null(optionsAccessor, nameof(optionsAccessor));

		Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));
	}

	/// <summary>
	/// Whether an active process is being tracked by this app instance.
	/// TODO: Include check for whether the process has been killed etc.
	/// </summary>
	public bool IsActive => Process != null;

	public string Name { get; }

	public WtqAppOptions Options => _optionsAccessor();

	public Process? Process { get; set; }

	// TODO: Track whether the app is currently open, has focus, etc.

	public string? ProcessDescription => Process == null
		? "<no process attached>"
		: $"[{Process.Id}] {Process.ProcessName}";

	public async Task AttachAsync(Process process)
	{
		Process = process;

		// TODO: Configurable.
		if (_opts.CurrentValue.GetTaskbarIconVisibilityForApp(Options) == TaskBarIconVisibility.AlwaysHidden)
		{
			_procService.SetTaskbarIconVisibility(process, false);
		}

		await CloseAsync(ToggleModifiers.Instant).ConfigureAwait(false);

		_log.LogInformation("Found process instance for app '{App}' with name '{ProcessName}' and Id '{ProcessId}'", Options, process.ProcessName, process.Id);
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

			await OpenAsync(ToggleModifiers.Instant).ConfigureAwait(false);

			_procService.SetTaskbarIconVisibility(Process, true);
		}
	}

	public WtqRect GetWindowRect()
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

	public async Task<bool> OpenAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		// If we have an active process attached, toggle it open.
		if (IsActive)
		{
			var ms = GetTimeMs(mods);

			_log.LogInformation("Opening app '{App}' in {Time}ms", this, ms);

			await _toggler.ToggleAsync(this, true, ms).ConfigureAwait(false);

			return true;
		}

		if (!IsActive && Options.AttachMode == AttachMode.Manual)
		{
			var pr = _procService.GetForegroundProcess();
			if (pr != null)
			{
				await AttachAsync(pr).ConfigureAwait(false);

				return true;
			}

			_log.LogWarning("ATTACH?!");
		}

		return false;
	}

	public override string ToString()
	{
		try
		{
			// TODO: Make extensions to safely pull process info without crashing.
			return $"[App:{Options}] [ProcessID:{Process?.Id}] {Process?.ProcessName ?? "<no process>"}";
		}
		catch (Exception ex)
		{
			return $"[App:{Options}] <no process>";
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
			var process = await _procFactory.GetProcessAsync(Options).ConfigureAwait(false);

			if (process == null)
			{
				_log.LogWarning("No process instances found for app '{App}'", Options);
				return;
			}

			await AttachAsync(process).ConfigureAwait(false);
		}

		// Update opacity.
		if (Process != null && IsActive)
		{
			_procService.SetTransparency(Process, _opts.CurrentValue.GetOpacityForApp(Options));
		}
	}

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
}