using System.Text.Json.Serialization;

namespace Wtq.Services.KWin.Dto;

/// <summary>
/// Represents a set of parameters sent to the KWin script, like for requesting the list of windows, or moving one, or setting its opacity.
/// </summary>
public class CommandInfo
{
	// public static readonly string[] CommandTypes =
	// [
	// 	"GET_WINDOW_LIST",
	// ];

	public CommandInfo()
	{
	}

	public CommandInfo(string type)
	{
		Type = type;
	}

	/// <summary>
	/// The command we want to execute, like "get window list" and "set window opacity".
	/// </summary>
	[JsonPropertyName("type")]
	public string Type { get; set; }

	/// <summary>
	/// Any parameters that accompany the command, like where to move a window to,
	/// or what opacity to set a window to.
	/// </summary>
	[JsonPropertyName("params")]
	public object Params { get; set; }

	/// <summary>
	/// Used to correlate any responses coming back from the KWin script.<br/>
	/// Note that not all commands result in a response.
	/// </summary>
	[JsonPropertyName("responderId")]
	public Guid ResponderId { get; set; } = Guid.NewGuid();

	public override string ToString() => $"[{Type}]";
}
