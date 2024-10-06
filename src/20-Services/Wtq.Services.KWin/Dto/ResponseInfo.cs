using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Tmds.DBus;
using Wtq.Configuration;
using Wtq.Events;

namespace Wtq.Services.KWin.DBus;

public class ResponseInfo
{
	[JsonPropertyName("responderId")]
	public Guid ResponderId { get; set; }

	[JsonPropertyName("params")]
	public JsonElement Params { get; set; }

	public T GetParamsAs<T>()
	{
		return Params.Deserialize<T>();
	}
}
