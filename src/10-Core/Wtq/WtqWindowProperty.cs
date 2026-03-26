namespace Wtq;

/// <summary>
/// Describes a property of a <see cref="WtqWindow"/>, that can be used to match windows on, or just informationally in the GUI.
/// </summary>
public class WtqWindowProperty(
	string name,
	Func<WtqWindow, object?> accessor,
	bool isVisible = true,
	int? width = null)
{
	private readonly ILogger _log = Log.For<WtqWindowProperty>();

	private Func<WtqWindow, object?> _accessor = Guard.Against.Null(accessor);

	public string Name { get; } = Guard.Against.NullOrWhiteSpace(name);

	public Func<WtqWindow, object?> Accessor => w =>
	{
		try
		{
			return _accessor(w);
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error fetching property '{Property}' from window '{Window}': {Message}", Name, this, ex.Message);
			return null;
		}
	};

	/// <summary>
	/// Whether the property should be visible by default (in the GUI).<br/>
	/// If "false", it needs to be toggled on manually. However, setting this to "false" saves space, and is useful for less relevant columns.
	/// </summary>
	public bool Visible { get; } = isVisible;

	/// <summary>
	/// Preferred width in the GUI.
	/// </summary>
	public int? Width { get; set; } = width;
}