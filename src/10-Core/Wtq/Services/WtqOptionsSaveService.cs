namespace Wtq.Services;

public class WtqOptionsSaveService(IPlatformService platform)
	: IWtqOptionsSaveService
{
	private readonly IPlatformService _platform = Guard.Against.Null(platform);

	public async Task SaveAsync(WtqOptions options)
	{
		Guard.Against.Null(options);

		await File.WriteAllTextAsync(_platform.PathToWtqConf, Write(options)).NoCtx();
	}

	public string Write(WtqOptions options)
	{
		Guard.Against.Null(options);

		options.PrepareForSave();

		return Json.Serialize(options);
	}
}