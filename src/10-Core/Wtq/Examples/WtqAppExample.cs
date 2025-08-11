using System.Runtime.InteropServices;

namespace Wtq.Examples;

/// <summary>
/// Example configuration for use in the UI, to quickly get started.
/// </summary>
public class WtqAppExample
{
	/// <summary>
	/// Name of the app the example is for.
	/// </summary>
	public required string Title { get; init; }

	/// <summary>
	/// Categories to bring some order to the "Examples" page.
	/// </summary>
	public required Category[] Categories { get; init; }

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

	/// <summary>
	/// An app can run in different ways, e.g. Windows/Linux, Native/Flatpak, etc.<br/>
	/// Flavors allow expressing each of these, so we can combine multiple methods in a single example, and the user
	/// can pick the one that they prefer/is available on their system.
	/// </summary>
	public required ICollection<WtqAppExampleFlavor> Flavors { get; init; }

	/// <summary>
	/// What operating systems the app runs on. Used for indication and filtering in the UI.
	/// </summary>
	public ICollection<OSPlatform> Os => Flavors.SelectMany(f => f.Os).Distinct().ToList();

	public bool IsLinux => Os.Contains(OSPlatform.Linux);

	public bool IsWindows => Os.Contains(OSPlatform.Windows);
}