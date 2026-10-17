namespace Wtq.Services;

public interface IWtqTargetScreenRectProvider
{
	/// <summary>
	/// Returns the rectangle of the screen that should be used to toggle the <paramref name="opts"/> to.
	/// </summary>
	Task<Rectangle> GetTargetScreenRectAsync(WtqAppOptions opts);
}
