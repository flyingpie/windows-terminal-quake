namespace Wtq.Services.UI;

/// <summary>
/// When state is spread across components, Blazor can lose track of what components should be
/// updated whenever state changes (especially with sibling components).
/// So to prevent the need for a load of event handlers everywhere, this <see cref="Notifier"/>
/// class implements a simple observable pattern, making it more straightforward to deal with lots
/// of cascading of state.
/// </summary>
public sealed class Notifier : IDisposable
{
	/// <summary>
	/// Keep track of everyone that wants to receive updates when state changes.
	/// </summary>
	private readonly List<Action> _l = new();

	/// <summary>
	/// Uses a timer to debounce updates.
	/// </summary>
	private System.Timers.Timer? _timer;

	public void Dispose()
	{
		_timer?.Dispose();
	}

	public void OnNotify(Action a)
	{
		_l.Add(a);
	}

	/// <summary>
	/// Tell everyone state has changed.
	/// </summary>
	public void Notify()
	{
		// Kill any existing timers (delaying the next update).
		_timer?.Dispose();

		// Set a new timer to run after a short while.
		_timer = new(300);
		_timer.Elapsed += (s, a) =>
		{
			((System.Timers.Timer)s!).Dispose();

			foreach (var l in _l)
			{
				l();
			}
		};
		_timer.Enabled = true;
		_timer.Start();
	}
}