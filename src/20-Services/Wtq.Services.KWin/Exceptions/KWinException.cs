namespace Wtq.Services.KWin.Exceptions;

public sealed class KWinException : Exception
{
	public KWinException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public KWinException(string message)
		: base(message)
	{
	}
}