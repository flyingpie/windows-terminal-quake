namespace Wtq.Configuration;

public class ProcessArgument
{
	public ProcessArgument()
	{
	}

	public ProcessArgument(string argument)
	{
		Argument = argument;
	}

	public string? Argument { get; set; }
}