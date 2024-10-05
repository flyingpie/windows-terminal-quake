using System.Text.Json;

namespace Wtq.Services.KWin;

public sealed class KWinResponseWaiter : IDisposable
{
	private readonly Action _onDone;
	private readonly TaskCompletionSource<string> _tcs = new();

	public KWinResponseWaiter(Guid id, Action onDone)
	{
		ArgumentNullException.ThrowIfNull(onDone);

		_onDone = onDone;

		Id = id;
	}

	public Guid Id { get; set; }

	public Task<string> Task => _tcs.Task;

	public void Dispose()
	{
		_onDone();
	}

	public async Task<TResult> GetResultAsync<TResult>(CancellationToken cancellationToken)
	{
		var respStr = await _tcs.Task.WaitAsync(cancellationToken).NoCtx();

		return JsonSerializer.Deserialize<TResult>(respStr)!;
	}

	public void SetResult(string result)
	{
		_tcs.TrySetResult(result);
	}
}