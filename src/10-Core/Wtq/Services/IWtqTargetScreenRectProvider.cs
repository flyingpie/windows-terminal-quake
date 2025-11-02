namespace Wtq.Services;

public interface IWtqTargetScreenRectProvider
{
	Task<Rectangle> GetTargetScreenRectAsync(WtqAppOptions opts);
}