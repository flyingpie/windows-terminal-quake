namespace Wtq.Win32.Native;

public struct Bounds : IEquatable<Bounds>
{
	public int Bottom { get; set; }

	public int Left { get; set; }

	public int Right { get; set; }

	public int Top { get; set; }

	public static bool operator !=(Bounds left, Bounds right)
	{
		return !(left == right);
	}

	public static bool operator ==(Bounds left, Bounds right)
	{
		return left.Equals(right);
	}

	public override readonly bool Equals(object? obj)
	{
		if (obj is not Bounds other)
		{
			return false;
		}

		return Equals(other);
	}

	public readonly bool Equals(Bounds other)
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