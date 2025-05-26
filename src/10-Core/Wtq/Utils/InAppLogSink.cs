using Serilog.Core;
using Serilog.Events;

namespace Wtq.Utils;

public class InAppLogSink : ILogEventSink
{
	public static InAppLogSink Instance { get; }
		= new();

	public ConcurrentQueue<LogEvent> Events { get; }
		= new();

	public Action? OnEvent { get; set; }

	public void Emit(LogEvent logEvent)
	{
		Events.Enqueue(logEvent);

		while (Events.Count > 2_000)
		{
			Events.TryDequeue(out _);
		}

		OnEvent?.Invoke();
	}
}