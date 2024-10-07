using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wtq.Services.KWin.Dto;

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