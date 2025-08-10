namespace Wtq.Utils;

public sealed class FsException : Exception
{
	public FsException(string message)
		: base(message)
	{
	}

	public FsException(string message, Exception ex)
		: base(message, ex)
	{
	}
}