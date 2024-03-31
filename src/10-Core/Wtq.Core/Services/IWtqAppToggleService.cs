namespace Wtq.Core.Services;

public interface IWtqAppToggleService
{
	Task ToggleAsync(WtqApp app, bool open, int durationMs);
}