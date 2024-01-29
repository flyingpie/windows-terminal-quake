namespace Wtq.Configuration;

public class CreateProcessOptions
{
	[NotNull]
	[Required]
	public string? FileName { get; set; }

	public IEnumerable<string> Arguments { get; set; } = Array.Empty<string>();
}