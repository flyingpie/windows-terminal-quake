using System.Runtime.InteropServices;

namespace Wtq.Examples;

public class WtqAppExampleFlavor
{
	public WtqAppExampleFlavor()
	{
		Factory = example => new()
		{
			Name = example.Title,
			FileName = FileName,
			ProcessName = ProcessName,
		};
	}

	public required string Name { get; init; }

	public string? FileName { get; set; }

	public string? ProcessName { get; set; }

	/// <summary>
	/// What operating systems the app runs on. Used for indication and filtering in the UI.
	/// </summary>
	public required ICollection<OSPlatform> Os { get; init; } = [OSPlatform.Linux, OSPlatform.Windows];

	/// <summary>
	/// Function that creates an instance of <see cref="WtqAppOptions" />, for use in the settings file.
	/// </summary>
	public Func<WtqAppExample, WtqAppOptions> Factory { get; init; }
}