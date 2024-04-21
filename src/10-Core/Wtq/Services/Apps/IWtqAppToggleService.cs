namespace Wtq.Services.Apps;

public interface IWtqAppToggleService
{
	Task ToggleAsync(WtqApp app, bool open, int durationMs);
}