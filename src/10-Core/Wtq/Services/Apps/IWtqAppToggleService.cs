namespace Wtq.Services.Apps;

public interface IWtqAppToggleService
{
	Task ToggleOnAsync(WtqApp app, ToggleModifiers mods);

	Task ToggleOffAsync(WtqApp app, ToggleModifiers mods);
}