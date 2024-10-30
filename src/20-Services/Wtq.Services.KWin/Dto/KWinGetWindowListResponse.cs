namespace Wtq.Services.KWin.Dto;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "MvdO: Used by deserializer.")]
public class KWinGetWindowListResponse
{
	[JsonPropertyName("windows")]
	public ICollection<KWinWindow> Windows { get; set; } = [];
}