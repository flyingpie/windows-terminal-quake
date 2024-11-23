namespace Wtq.Services.KWin.Dto;

/// <summary>
/// Represents a set of parameters sent to the KWin script, like for requesting the list of windows, or moving one, or setting its opacity.
/// </summary>
public class CommandInfo(
	string type)
{
	public static readonly CommandInfo NoOp
		= new("NOOP");

	public static readonly CommandInfo Stopping
		= new("STOPPING");

	/// <summary>
	/// The command we want to execute, like "get window list" and "set window opacity".
	/// </summary>
	[JsonPropertyName("type")]
	public string Type { get; } = Guard.Against.NullOrWhiteSpace(type);

	/// <summary>
	/// Any parameters that accompany the command, like where to move a window to,
	/// or what opacity to set a window to.
	/// </summary>
	[JsonPropertyName("params")]
	public object? Params { get; set; }

	/// <summary>
	/// Used to correlate any responses coming back from the KWin script.<br/>
	/// </summary>
	[JsonPropertyName("responderId")]
	public Guid ResponderId { get; } = Guid.NewGuid();

	public override string ToString() => $"[{Type}]";
}