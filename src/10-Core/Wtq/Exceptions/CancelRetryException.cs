namespace Wtq.Exceptions;

public sealed class CancelRetryException : Exception
{
	public CancelRetryException()
	{
	}

	public CancelRetryException(string message)
		: base(message)
	{
	}

	public CancelRetryException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}