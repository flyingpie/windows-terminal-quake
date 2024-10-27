namespace Wtq.Services;

public interface IAsyncInitializable
{
	int InitializePriority => 0;

	Task InitializeAsync();
}