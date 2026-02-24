namespace Wtq.Configuration;

/// <summary>
/// Options related to the HTTP API, that can be used to control WTQ programmatically.
/// </summary>
public class WtqApiOptions
{
	/// <summary>
	/// Whether the HTTP API is enabled at all.
	/// </summary>
	public bool Enable { get; set; }

	/// <summary>
	/// The addresses on which the HTTP API will listen.<br/>
	/// On Windows, this defaults to a named pipe, with name "wtq".<br/>
	/// On Linux, this defaults to a Unix socket, at /run/user/USERNAME/wtq/wtq.sock.<br/>
	/// <br/>
	/// A regular socket can also be used, e.g. "http://127.0.0.1:7997".
	/// </summary>
	public ICollection<string>? Urls { get; set; }
}