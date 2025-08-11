namespace Wtq.Examples;

public class Category(string category)
{
	public string Name { get; } = Guard.Against.NullOrWhiteSpace(category);

	public int Priority { get; set; }
}