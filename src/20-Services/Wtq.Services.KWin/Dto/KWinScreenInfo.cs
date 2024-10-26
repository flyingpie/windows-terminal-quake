namespace Wtq.Services.KWin.Dto;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "MvdO: Used by deserializer.")]
public class KWinScreenInfo
{
	public string? Name { get; set; }

	public bool IsEnabled { get; set; }

	public Rectangle Geometry { get; set; }
}