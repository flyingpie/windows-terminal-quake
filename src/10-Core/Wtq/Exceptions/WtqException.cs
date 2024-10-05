namespace Wtq.Exceptions;

public sealed class WtqException : Exception
{
	public WtqException()
	{
	}

	public WtqException(string message)
		: base(message)
	{
	}

	public WtqException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}