using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Tmds.DBus;
using Wtq.Configuration;
using Wtq.Events;

namespace Wtq.Services.KWin.Dto;

public class CommandInfo
{
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
