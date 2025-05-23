namespace Wtq;

public class WtqWindowProperty
{
	public WtqWindowProperty(string name, Func<WtqWindow, object?> accessor)
	{
		Name = Guard.Against.NullOrWhiteSpace(name);
		Accessor = Guard.Against.Null(accessor);
	}

	public WtqWindowProperty()
	{

	}

	public string Name { get; set; }

	public string? Description { get; set; }

	public string? SettingsPropertyName { get; set; }

	public int? ColumnWidth { get; set; }

	public Func<WtqWindow, object?> Accessor { get; set; }
}