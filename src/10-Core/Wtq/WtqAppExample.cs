using System.Runtime.InteropServices;

namespace Wtq;

public class WtqAppExample
{
	public required string Title { get; init; }

	public required string Description { get; init; }

	public required Func<WtqAppOptions> Factory { get; init; }

	public required ICollection<OSPlatform> Os { get; init; }

	public Uri? Link { get; set; }

	public string? Image { get; set; }

	public string? Icon { get; set; }
}