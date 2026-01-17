// using Microsoft.Extensions.Logging;
//
// namespace Wtq.Services.UI;
//
// public class WtqUIInvoker : IDisposable, IWtqUIService
// {
// 	private readonly ILogger _log = Log.For<WtqUIInvoker>();
// 	private bool _isClosing;
//
// 	public Action<Action>? Action { get; set; }
//
// 	public void Dispose()
// 	{
// 		_isClosing = true;
// 	}
//
// 	public void RunOnUIThread(Action action)
// 	{
// 		if (_isClosing)
// 		{
// 			_log.LogWarning("UI is stopping, skipping '{Name}' action", nameof(RunOnUIThread));
// 			return;
// 		}
//
// 		try
// 		{
// 			Action?.Invoke(action);
// 		}
// 		catch (Exception ex)
// 		{
// 			_log.LogWarning(ex, "Error running action on UI thread: {Message}", ex.Message);
// 		}
// 	}
// }