using Wtq.Data;

namespace Wtq;

public abstract class WtqWindow : IEquatable<WtqWindow>
{
	public abstract int Id { get; }

	public abstract bool IsValid { get; }

	public abstract string? Name { get; }

	public abstract WtqRect WindowRect { get; }

	public static bool operator !=(WtqWindow left, WtqWindow right)
	{
		return !(left == right);
	}

	public static bool operator ==(WtqWindow left, WtqWindow right)
	{
		if (ReferenceEquals(left, right))
		{
			return true;
		}

		if (ReferenceEquals(left, null))
		{
			return false;
		}

		if (ReferenceEquals(right, null))
		{
			return false;
		}

		return left.Equals(right);
	}

	public abstract void BringToForeground();

	public override bool Equals(object? obj)
	{
		return Equals(obj as WtqWindow);
	}

	public bool Equals(WtqWindow? other)
	{
		if (ReferenceEquals(other, null))
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return Id == other.Id;
	}

	public override int GetHashCode()
	{
		return Id;
	}

	public abstract bool Matches(WtqAppOptions opts);

	public abstract void MoveTo(WtqRect rect, bool repaint = true);

	public abstract void SetAlwaysOnTop(bool isAlwaysOnTop);

	public abstract void SetTaskbarIconVisible(bool isVisible);

	public abstract void SetTransparency(int transparency);
}