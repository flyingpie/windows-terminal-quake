using System.Collections.Generic;

namespace Wtq.Services.UI;

public sealed class Notifier : IDisposable
{
	private readonly List<Action> _l = new();

	private System.Timers.Timer? _timer;

	public void Dispose()
	{
		_timer?.Dispose();
	}

	public void OnNotify(Action a)
	{
		_l.Add(a);
	}

	public void Notify()
	{
		_timer?.Dispose();
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