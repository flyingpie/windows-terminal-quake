namespace Wtq.Services.Stubs;

public class StubWtqWindowService : IWtqWindowService
{
	public Task CreateAsync(WtqAppOptions opts, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task<ICollection<WtqWindow>> FindWindowsAsync(WtqAppOptions opts, CancellationToken cancellationToken)
	{
		return Task.FromResult<ICollection<WtqWindow>>([]);
	}

	public Task<WtqWindow?> GetForegroundWindowAsync(CancellationToken cancellationToken)
	{
		return Task.FromResult<WtqWindow?>(null);
	}

	public List<WtqWindowProperty> GetWindowProperties()
	{
		return [];
	}

	public Task<ICollection<WtqWindow>> GetWindowsAsync(CancellationToken cancellationToken)
	{
		return Task.FromResult<ICollection<WtqWindow>>([]);
	}
}