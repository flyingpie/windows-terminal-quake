namespace Wtq.Host.Linux.E2ETest;

/// <summary>
/// Subset of what KWin's "getWindowInfo" DBus method returns.
/// </summary>
public sealed record WindowInfo(int Pid, string Caption, Rectangle Rect);
