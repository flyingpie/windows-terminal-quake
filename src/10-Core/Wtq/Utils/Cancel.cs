namespace Wtq.Utils;

/// <summary>
/// Cancellation token creation utils.
/// </summary>
public static class Cancel
{
	public static CancellationToken After(TimeSpan timeSpan) => new CancellationTokenSource(timeSpan).Token;
}