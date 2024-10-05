namespace Wtq.Services;

public interface IWtqProcessFactory
{
	Task<WtqWindow?> GetProcessAsync(WtqAppOptions opts);
}