using Wtq.Data;

namespace Wtq;

public abstract class WtqWindow : IEquatable<WtqWindow>
{
	public abstract int Id { get; }

	/// <summary>
	/// Whether the window handle is still pointing to an existing window.
	/// </summary>
	public abstract bool IsValid { get; }

	public abstract string? Name { get; }

	public abstract WtqRect WindowRect { get; }

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

	public abstract void BringToForeground();

	public abstract bool Matches(WtqAppOptions opts);

	public abstract void MoveTo(WtqRect rect, bool repaint = true);

	public abstract void SetAlwaysOnTop(bool isAlwaysOnTop);

	public abstract void SetTaskbarIconVisible(bool isVisible);

	public abstract void SetTransparency(int transparency);

	public override string ToString() => $"[{Id}] {Name}";
}