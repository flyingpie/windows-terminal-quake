using Wtq.Events;

namespace Wtq.Services;

/// <summary>
/// Receives raw hot key events from a platform-specific service, and converts them to more
/// specific events, such as <see cref="WtqAppToggledEvent"/>.
/// </summary>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "MvdO: Flag interface.")]
public interface IWtqHotKeyService
{
}