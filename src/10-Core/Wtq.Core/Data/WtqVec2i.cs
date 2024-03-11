namespace Wtq.Core.Data;

public struct WtqVec2I : IEquatable<WtqVec2I>
{
	public int X { get; set; }

	public int Y { get; set; }

	public static bool operator !=(WtqVec2I left, WtqVec2I right)
	{
		return !(left == right);
	}

	public static bool operator ==(WtqVec2I left, WtqVec2I right)
	{
		return left.Equals(right);
	}

	public override readonly bool Equals(object obj)
	{
		if (obj is not WtqVec2I other)
		{
			return false;
		}

		return Equals(other);
	}

	public readonly bool Equals(WtqVec2I other)
	{
		return
			X == other.X &&
			Y == other.Y;
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}
}