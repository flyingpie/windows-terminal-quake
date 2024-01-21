namespace Wtq.Exceptions;

public sealed class WindowsTerminalQuakeException : Exception
{
	public WindowsTerminalQuakeException()
	{
	}

	public WindowsTerminalQuakeException(string message)
		: base(message)
	{
	}

	public WindowsTerminalQuakeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}