namespace Wtq.Utils;
public abstract class WtqBackgroundService : BackgroundService
{
	public override Task StartAsync(CancellationToken cancellationToken)
	{
		return base.StartAsync(cancellationToken);
	}()
}

