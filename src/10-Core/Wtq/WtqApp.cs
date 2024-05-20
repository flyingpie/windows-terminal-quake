using Wtq.Data;
using Wtq.Services;
using Wtq.Services.Apps;

namespace Wtq;

/// <summary>
/// An "app" represents a single process that can be toggled (such as Windows Terminal).<br/>
/// It tracks its own state, and does not necessarily have a process attached.
/// </summary>
// TODO: Track whether the app is currently open, has focus, etc.
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
		_opts = Guard.Against.Null(opts);
		_procFactory = Guard.Against.Null(procFactory);
		_procService = Guard.Against.Null(procService);
		_toggler = Guard.Against.Null(toggler);
		_optionsAccessor = Guard.Against.Null(optionsAccessor);

		Name = Guard.Against.NullOrWhiteSpace(name);
	}

	/// <summary>
	/// Whether an active process is being tracked by this app instance.
	/// TODO: Include check for whether the process has been killed etc.
	/// </summary>
	public bool IsActive => Process != null;

	public string Name { get; }

	public WtqAppOptions Options => _optionsAccessor();

	public WtqWindow? Process { get; private set; }

	public string? ProcessDescription => Process == null
		? "<no process attached>"
		: Process.ToString();

	public async Task AttachAsync(WtqWindow process)
	{
		Guard.Against.Null(process);

		Process = process;

		// TODO: Configurable.
		if (_opts.CurrentValue.GetTaskbarIconVisibilityForApp(Options) == TaskBarIconVisibility.AlwaysHidden)
		{
			process.SetTaskbarIconVisible(false);
		}

		await CloseAsync(ToggleModifiers.Instant).ConfigureAwait(false);

		_log.LogInformation("Found process instance for app '{App}': '{Process}'", Options, process);
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

		Process.BringToForeground();
	}

	public async Task CloseAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		_log.LogInformation("Closing app '{App}'", this);

		await _toggler.ToggleOffAsync(this, mods).ConfigureAwait(false);
	}

	public async ValueTask DisposeAsync()
	{
		// TODO: Add ability to close attached processes when app closes.
		if (Process != null)
		{
			var bounds = Process.WindowRect; // TODO: Restore to original position (when we got a hold of the process).
			bounds.Width = 1280;
			bounds.Height = 800;
			bounds.X = 10;
			bounds.Y = 10;

			_log.LogInformation("Restoring process '{Process}' to its original bounds of '{Bounds}'", ProcessDescription, bounds);

			await OpenAsync(ToggleModifiers.Instant).ConfigureAwait(false);

			Process.SetTaskbarIconVisible(true);
		}
	}

	[SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "MvdO: May throw an exception, which we don't want to do in a property.")]
	public WtqRect GetWindowRect()
	{
		if (Process == null)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		return Process.WindowRect;
	}

	public void MoveWindow(WtqRect rect)
	{
		if (Process == null)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		Process.MoveTo(rect: rect);
	}

	public async Task<bool> OpenAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		// If we have an active process attached, toggle it open.
		if (IsActive)
		{
			_log.LogInformation("Opening app '{App}'", this);

			await _toggler.ToggleOnAsync(this, mods).ConfigureAwait(false);

			return true;
		}

		if (!IsActive && Options.AttachMode == AttachMode.Manual)
		{
			var pr = _procService.GetForegroundWindow();
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
			return $"[App:{Options}] {Process?.ToString() ?? "<no process>"}";
		}
		catch
		{
			return $"[App:{Options}] <no process>";
		}
	}

	/// <summary>
	/// Updates the state of the <see cref="WtqApp"/> object to reflect running processes on the system.
	/// </summary>
	public async Task UpdateAsync()
	{
		// TODO: Only update the window handles on app start, and then on hot key pressed.
		// Check that if we have a process handle, the process is still active.
		if (Process is { IsValid: false })
		{
			_log.LogInformation("Process '{ProcessName}' exited, releasing handle", Process);
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

		if (Process != null && IsActive)
		{
			// Always on top.
			Process.SetAlwaysOnTop(_opts.CurrentValue.GetAlwaysOnTopForApp(Options));

			// Opacity.
			Process.SetTransparency(_opts.CurrentValue.GetOpacityForApp(Options));
		}
	}
}