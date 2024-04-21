namespace Wtq.Configuration;

/// <summary>
/// Defines the options for a single toggleable app (eg. Windows Terminal, some other terminal, a file browser, etc.).
/// </summary>
public class WtqAppOptions
{
	/// <summary>
	/// Command-line arguments that should be passed to the app when it's started.<br/>
	/// Note that this only applies when using an <see cref="AttachMode"/> that starts the app.
	/// </summary>
	public string? Arguments { get; set; }

	/// <inheritdoc cref="WtqOptions.AttachMode"/>
	public AttachMode? AttachMode { get; set; }

	/// <summary>
	/// Path to an executable that should be run when we're starting an app instance ourselves.
	/// </summary>
	[NotNull]
	[Required]
	public string? FileName { get; set; }

	/// <inheritdoc cref="WtqOptions.HorizontalAlign"/>
	public HorizontalAlign? HorizontalAlign { get; set; }

	/// <inheritdoc cref="WtqOptions.HorizontalScreenCoverage"/>
	public float? HorizontalScreenCoverage { get; set; }

	public IEnumerable<HotKeyOptions> HotKeys { get; set; } = [];

	/// <inheritdoc cref="WtqOptions.MonitorIndex"/>
	public int? MonitorIndex { get; set; }

	/// <summary>
	/// A logical name for the app, used to identify it across config reloads.<br/>
	/// Appears in logs.
	/// </summary>
	[NotNull]
	[Required]
	public string? Name { get; set; }

	/// <inheritdoc cref="WtqOptions.Opacity"/>
	public int? Opacity { get; set; }

	/// <inheritdoc cref="WtqOptions.PreferMonitor"/>
	public PreferMonitor? PreferMonitor { get; set; }

	/// <summary>
	/// The name of the process to look for, when searching for an existing app instance.
	/// </summary>
	public string? ProcessName { get; set; }

	/// <inheritdoc cref="WtqOptions.TaskBarIconVisibility"/>
	public TaskBarIconVisibility? TaskbarIconVisibility { get; set; }

	/// <inheritdoc cref="WtqOptions.VerticalOffset"/>
	public int? VerticalOffset { get; set; }

	/// <inheritdoc cref="WtqOptions.VerticalScreenCoverage"/>
	public float? VerticalScreenCoverage { get; set; }

	public bool Filter(Process process, bool isStartedByWtq)
	{
		ArgumentNullException.ThrowIfNull(process);

		if (process.MainWindowHandle == nint.Zero)
		{
			return false;
		}

		// TODO: Make some notes about webbrowsers being a pain with starting a new process.
		try
		{
			if (process.MainModule == null)
			{
				return false;
			}

			if (process.MainWindowHandle == nint.Zero)
			{
				return false;
			}

			// TODO: Handle extensions either or not being there.
			var fn1 = Path.GetFileNameWithoutExtension(ProcessName ?? FileName);
			var fn2 = Path.GetFileNameWithoutExtension(process.MainModule.FileName);

			if (!fn1.Equals(fn2, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			if (isStartedByWtq)
			{
				if (!process.StartInfo.Environment.TryGetValue("WTQ_START", out var val))
				{
					return false;
				}

				if (string.IsNullOrWhiteSpace(val))
				{
					return false;
				}

				if (!val.Equals(Name, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}

			return true;
		}
		catch
		{
			// TODO: Remove try/catch, use safe property access methods on "Process" instead.
		}

		return false;
	}

	public bool HasHotKey(Keys key, KeyModifiers modifiers)
	{
		return HotKeys.Any(hk => hk.Key == key && hk.Modifiers == modifiers);
	}

	public override string ToString() => Name;
}