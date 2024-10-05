namespace Wtq;

public abstract class WtqWindow : IEquatable<WtqWindow>
{
	public abstract int Id { get; }

	/// <summary>
	/// Whether the window handle is still pointing to an existing window.
	/// </summary>
	public abstract bool IsValid { get; }

	public abstract string? Name { get; }

	/// <summary>
	/// The rectangle of the window itself.
	/// </summary>
	public abstract Rectangle WindowRect { get; }

	public static bool operator ==(WtqWindow? left, WtqWindow? right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(WtqWindow? left, WtqWindow? right)
	{
		return !Equals(left, right);
	}

	public bool Equals(WtqWindow? other)
	{
		if (ReferenceEquals(null, other))
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return Id == other.Id;
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		return obj.GetType() == GetType() && Equals((WtqWindow)obj);
	}

	public override int GetHashCode()
	{
		return Id;
	}

	public abstract Task BringToForegroundAsync();

	public abstract bool Matches(WtqAppOptions opts);

	public abstract Task MoveToAsync(Rectangle rect, bool repaint = true);

	public abstract Task SetAlwaysOnTopAsync(bool isAlwaysOnTop);

	public abstract Task SetTaskbarIconVisibleAsync(bool isVisible);

	public abstract Task SetTransparencyAsync(int transparency);

	public abstract Task SetVisibleAsync(bool isVisible);

	public override string ToString() => $"[{Id}] {Name}";
}