namespace Wtq.Core.Data;

public struct WtqBounds : IEquatable<WtqBounds>
{
	public int Bottom { get; set; }

	public int Left { get; set; }

	public int Right { get; set; }

	public int Top { get; set; }

	public static bool operator ==(WtqBounds left, WtqBounds right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(WtqBounds left, WtqBounds right)
	{
		return !(left == right);
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is not WtqBounds other)
		{
			return false;
		}

		return Equals(other);
	}

	public readonly bool Equals(WtqBounds other)
	{
		return
			Bottom == other.Bottom &&
			Left == other.Left &&
			Right == other.Right &&
			Top == other.Top;
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(Bottom, Left, Right, Top);
	}
}