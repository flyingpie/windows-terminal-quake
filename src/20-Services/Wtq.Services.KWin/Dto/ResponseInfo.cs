using System.Text.Json;
using System.Text.Json.Serialization;
using Wtq.Services.KWin.Exceptions;

namespace Wtq.Services.KWin.Dto;

public class ResponseInfo
{
	private Exception? _exc;

	[JsonPropertyName("cmdType")]
	public string? CommandType { get; set; }

	[JsonPropertyName("responderId")]
	public Guid ResponderId { get; set; }

	[JsonPropertyName("params")]
	public JsonElement Params { get; set; }

	[JsonPropertyName("exception_message")]
	public string? ExceptionMessage { get; set; }

	public Exception? Exception
	{
		get
		{
			if (_exc == null && !string.IsNullOrWhiteSpace(ExceptionMessage))
			{
				_exc = new KWinException(ExceptionMessage);
			}

			return _exc;
		}
	}

	public T? GetParamsAs<T>()
	{
		// TODO: Error handling, with print of json.
		return Params.Deserialize<T>();
	}

	public override string ToString() => $"[{CommandType}] exc:{(ExceptionMessage)}";
}