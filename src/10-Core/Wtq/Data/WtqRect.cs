namespace Wtq.Data;

public struct WtqRect : IEquatable<WtqRect>
{
	public static readonly WtqRect Default = new()
	{
		X = 100,
		Y = 100,
		Height = 1000,
		Width = 1000,
	};

	public int Height { get; set; }

	public int Width { get; set; }

	public int X { get; set; }

	public int Y { get; set; }

	public static bool operator !=(WtqRect left, WtqRect right)
	{
		return !(left == right);
	}

	public static bool operator ==(WtqRect left, WtqRect right)
	{
		return left.Equals(right);
	}

	public readonly bool Contains(WtqVec2I pos)
	{
		return
			X < pos.X &&
			Y < pos.Y &&
			X + Width > pos.X &&
			Y + Height > pos.Y;
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is not WtqRect other)
		{
			return false;
		}

		return Equals(other);
	}

	public readonly bool Equals(WtqRect other)
	{
		return
			X == other.X &&
			Y == other.Y &&
			Width == other.Width &&
			Height == other.Height;
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(Height, Width, X, Y);
	}

	public override readonly string ToString() => $"X:{X}, Y:{Y}, Width:{Width}, Height:{Height}";
}