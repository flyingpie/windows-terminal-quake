namespace Wtq.Services;

public interface IWtqProcessFactory
{
	Task<Process?> GetProcessAsync(WtqAppOptions opts);
}