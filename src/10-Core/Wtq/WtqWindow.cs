namespace Wtq;

/// <summary>
/// Represents a single application window, provides information about it such as position and size, and allows operations such as moving and setting opacity.
/// </summary>
public abstract class WtqWindow
	: IEquatable<WtqWindow>, IEqualityComparer<WtqWindow>
{
	/// <summary>
	/// A string that uniquely identifies this window across the system.
	/// </summary>
	public abstract string Id { get; }

	/// <summary>
	/// Whether the window handle is still pointing to an existing window.
	/// </summary>
	public abstract bool IsValid { get; }

	/// <summary>
	/// A descriptive name of the window, often (close to) the process name.
	/// </summary>
	public abstract string? Name { get; }

	public abstract string? Title { get; }

	public abstract Task BringToForegroundAsync();

	/// <summary>
	/// The rectangle of the window itself, includes both position and size.
	/// </summary>
	public abstract Task<Rectangle> GetWindowRectAsync();

	public abstract bool Matches(WtqAppOptions opts);

	public abstract Task MoveToAsync(Point location);

	// TODO: Warn or disallow sizes below a certain threshold? Seems to happend occasionally.
	public abstract Task ResizeAsync(Size size);

	public abstract Task SetAlwaysOnTopAsync(bool isAlwaysOnTop);

	public abstract Task SetTaskbarIconVisibleAsync(bool isVisible);

	public abstract Task SetTransparencyAsync(int transparency);

	public abstract Task SetWindowTitleAsync(string title);

	public abstract Task UpdateAsync();

	public override string ToString() => $"[{Id}] {Name}";

	#region Equality

	public static bool operator ==(WtqWindow? left, WtqWindow? right)
	{
		if (ReferenceEquals(left, right))
		{
			return true;
		}

		if (ReferenceEquals(null, left))
		{
			return false;
		}

		if (ReferenceEquals(null, right))
		{
			return false;
		}

		return left.Equals(right);
	}

	public static bool operator !=(WtqWindow? left, WtqWindow? right) => !(left == right);

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

		return Id.Equals(other.Id, StringComparison.Ordinal);
	}

	public bool Equals(WtqWindow? x, WtqWindow? y)
	{
		if (ReferenceEquals(x, y))
		{
			return true;
		}

		if (x is null)
		{
			return false;
		}

		if (y is null)
		{
			return false;
		}

		if (x.GetType() != y.GetType())
		{
			return false;
		}

		return x.Id.Equals(y.Id, StringComparison.Ordinal);
	}

	public int GetHashCode(WtqWindow obj)
	{
		Guard.Against.Null(obj);

		return obj.Id.GetHashCode(StringComparison.Ordinal);
	}

	public override int GetHashCode() => Id.GetHashCode(StringComparison.Ordinal);

	#endregion
}