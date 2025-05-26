using System.Runtime.InteropServices;

namespace Wtq;

/// <summary>
/// Example configuration for use in the UI, to quickly get started.
/// </summary>
public class WtqAppExample
{
	public WtqAppExample()
	{
		Factory = () => new WtqAppOptions()
		{
			Name = Title,
			FileName = FileName,
		};
	}

	/// <summary>
	/// Name of the app the example is for.
	/// </summary>
	public required string Title { get; init; }

	/// <summary>
	/// Categories to bring some order to the "Examples" page.
	/// </summary>
	public required string[] Categories { get; init; }

	/// <summary>
	/// Short description of what the app does.
	/// </summary>
	public required string Description { get; init; }

	/// <summary>
	/// Link to the app website.
	/// </summary>
	public Uri? Link { get; set; }

	/// <summary>
	/// Path to a thumbnail to show in the UI.
	/// </summary>
	public string? Image { get; set; }

	public string? FileName { get; set; }

	/// <summary>
	/// What operating systems the app runs on. Used for indication and filtering in the UI.
	/// </summary>
	public required ICollection<OSPlatform> Os { get; init; }

	public bool IsLinux => Os.Contains(OSPlatform.Linux);

	public bool IsWindows => Os.Contains(OSPlatform.Windows);

	/// <summary>
	/// Function that creates an instance of <see cref="WtqAppOptions" />, for use in the settings file.
	/// </summary>
	public Func<WtqAppOptions> Factory { get; init; }
}