namespace Wtq.Services.Win32.Native;

[Flags]
[SuppressMessage("Critical Code Smell", "S2346:Flags enumerations zero-value members should be named \"None\"", Justification = "MvdO: In line with MSDN.")]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "MvdO: In line with MSDN.")]
public enum SendMessageTimeoutFlags : uint
{
#pragma warning disable CA1707 // Identifiers should not contain underscores // MvdO: In line with MSDN.

	/// <summary>
	/// The calling thread is not prevented from processing other requests while waiting for the function to return.
	/// </summary>
	SMTO_NORMAL = 0x0,

	/// <summary>
	/// Prevents the calling thread from processing any other requests until the function returns.
	/// </summary>
	SMTO_BLOCK = 0x1,

	/// <summary>
	/// The function returns without waiting for the time-out period to elapse if the receiving thread appears to not respond or "hangs".
	/// </summary>
	SMTO_ABORTIFHUNG = 0x2,

	/// <summary>
	/// The function does not enforce the time-out period as long as the receiving thread is processing messages.
	/// </summary>
	SMTO_NOTIMEOUTIFNOTHUNG = 0x8,

	/// <summary>
	/// The function should return 0 if the receiving window is destroyed or its owning thread dies while the message is being processed.
	/// </summary>
	SMTO_ERRORONEXIT = 0x20,

#pragma warning restore CA1707 // Identifiers should not contain underscores
}