namespace Wtq.Services.KWin.Dto;

public class KWinScreenInfo
{
	public string? Name { get; set; }

	public bool IsEnabled { get; set; }

	public Rectangle Geometry { get; set; }
}