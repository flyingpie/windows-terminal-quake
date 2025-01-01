namespace Wtq.Utils;

public abstract class WtqDisposable : IAsyncDisposable, IDisposable
{
	public static void OnStart()
	{

	}

	public static void OnStop()
	{

	}

	public void Dispose()
	{
		// TODO release managed resources here
	}

	public async ValueTask DisposeAsync()
	{
		// TODO release managed resources here
	}
}