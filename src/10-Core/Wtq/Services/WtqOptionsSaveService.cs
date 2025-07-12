namespace Wtq.Services;

public class WtqOptionsSaveService
{
	public async Task SaveAsync(WtqOptions options)
	{
		// await File.WriteAllTextAsync(WtqOptionsPath.Instance.Path, Write(options)).NoCtx();
	}

	public string Write(WtqOptions options)
	{
		options.PrepareForSave();

		return Json.Serialize(options);
	}
}