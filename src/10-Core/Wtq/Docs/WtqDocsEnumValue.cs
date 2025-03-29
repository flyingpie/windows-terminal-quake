namespace Wtq.Docs;

public class WtqDocsEnumValue(EnumValue val)
{
	public object Value => val.Value;

	public string? DisplayName => val.DisplayName;

	public string? Doc => val.DocElement?.Descendants("summary").FirstOrDefault()?.Value.Replace("    ", "").Trim(); // TODO: This may strip element within <summary>, like <strong>.
}