using System.ComponentModel.DataAnnotations;
using WindowsTerminalQuake.TerminalProcessProviders;

namespace WindowsTerminalQuake.Settings;

public sealed class App
{
	[Required]
	public ExistingProcessInfo ExistingProcess { get; set; }

	[Required]
	public NewProcessInfo NewProcess { get; set; }

	/// <summary>
	/// <para>The keys that can be used to toggle the terminal.</para>
	/// <para>See "Hotkeys" for possible values.</para>
	/// </summary>
	public IReadOnlyCollection<Hotkey>? Hotkeys { get; set; } = Array.Empty<Hotkey>();

	/// <summary>
	/// TODO
	/// </summary>
	//public string ProcessProvider { get; set; } = nameof(GenericProcessProvider);

	public IProcessProvider CreateProcessProvider()
	{
		return new GenericProcessProvider(this);
	}

	public bool Filter(Process process)
	{
		return ExistingProcess.Filter(process);
	}

	public override string ToString()
	{
		return NewProcess?.FileName ?? ExistingProcess?.ProcessName ?? "<unknown>";
	}
}

public sealed class NewProcessInfo
{
	public IEnumerable<string> Arguments { get; set; }

	public string FileName { get; set; }

	public Process Create()
	{
		var process = new Process
		{
			StartInfo = new ProcessStartInfo
			{
				FileName = FileName,
				Arguments = string.Join(" ", Arguments ?? Array.Empty<string>()),
				UseShellExecute = false
			},
		};

		return process;
	}
}

public sealed class ExistingProcessInfo
{
	public string? ProcessName { get; set; }

	public bool Filter(Process process)
	{
		if (!string.IsNullOrWhiteSpace(ProcessName))
		{
			return process.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase);
		}

		return false;
	}
}