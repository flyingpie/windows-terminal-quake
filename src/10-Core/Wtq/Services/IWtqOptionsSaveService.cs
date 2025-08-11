namespace Wtq.Services;

public interface IWtqOptionsSaveService
{
	Task SaveAsync(WtqOptions options);

	string Write(WtqOptions options);
}