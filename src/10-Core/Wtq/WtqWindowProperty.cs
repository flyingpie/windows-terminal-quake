namespace Wtq;

public class WtqWindowProperty(string name, Func<WtqWindow, object?> accessor)
{
	public string Name { get; } = Guard.Against.NullOrWhiteSpace(name);

	public Func<WtqWindow, object?> Accessor { get; } = Guard.Against.Null(accessor);
}