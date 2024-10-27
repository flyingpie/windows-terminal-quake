namespace Wtq.Services;

public interface IAsyncInitializable
{
	int Priority => 0;

	Task InitializeAsync();
}