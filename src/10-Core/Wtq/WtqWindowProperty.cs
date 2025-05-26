namespace Wtq;

/// <summary>
/// Describes a property of a <see cref="WtqWindow"/>, that can be used to match windows on, or just informationally in the GUI.<br/>
/// </summary>
public class WtqWindowProperty
{
	public WtqWindowProperty(string name, Func<WtqWindow, object?> accessor)
	{
		Name = Guard.Against.NullOrWhiteSpace(name);
		Accessor = Guard.Against.Null(accessor);
	}

	public string Name { get; set; }

	public Func<WtqWindow, object?> Accessor { get; set; }
}