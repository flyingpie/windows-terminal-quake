namespace Wtq.Core.UnitTest.Utils;

[TestClass]
public class MathUtilsTest
{
	[TestMethod]
	public void CeilI()
	{
		Assert.AreEqual(85, 84.2f.CeilI());
	}

	[TestMethod]
	public void CenterInRectangle()
	{
		Assert.AreEqual(
			new Point(400, 250),
			new Size(800, 600).CenterInRectangle(new Rectangle(200, 100, 1200, 900))
		);
	}

	/// csharpier-ignore-start
	/// <summary>
	///   <----50---->
	///   ------------  ------------  ------------
	///   |  src - x |  |  src - v |  |  src - x |
	///   ------------  ------------  ------------
	///             <--------100------->
	///             --------------------
	///             |        dst       |
	///             --------------------
	///             | x=100            | x=200
	/// </summary>
	[TestMethod]
	//       srcX	srxWidth	dstX	dstWidth	isWithin
	[DataRow(0, 50, 100, 200, false)] // Left, no overlap.
	[DataRow(10, 50, 100, 200, false)] // Left, no overlap.
	[DataRow(0, 100, 100, 200, false)] // Left, brushing against.
	[DataRow(10, 100, 100, 200, false)] // Left side, partial overlap.
	//
	[DataRow(100, 100, 100, 200, true)] // Exact overlap
	[DataRow(125, 50, 100, 200, true)] // Within
	//
	[DataRow(310, 50, 100, 200, false)] // Right, no overlap.
	[DataRow(300, 50, 100, 200, false)] // Right, brushing against.
	[DataRow(90, 50, 100, 200, false)] // Right side, partial overlap.
	/// csharpier-ignore-end
	public void IsWithinHorizontal(int srcX, int srcWidth, int dstX, int dstWidth, bool expected)
	{
		Assert.AreEqual(
			expected,
			new Rectangle(srcX, 0, srcWidth, 0).IsWithinHorizontal(
				new Rectangle(dstX, 0, dstWidth, 0)
			)
		);
	}

	[TestMethod]
	public void Lerp()
	{
		Assert.AreEqual(
			new Point(350, 225),
			MathUtils.Lerp(new Point(200, 100), new Point(800, 600), .25f)
		);
		Assert.AreEqual(
			new Point(500, 350),
			MathUtils.Lerp(new Point(200, 100), new Point(800, 600), .50f)
		);
		Assert.AreEqual(
			new Point(650, 475),
			MathUtils.Lerp(new Point(200, 100), new Point(800, 600), .75f)
		);
	}

	[TestMethod]
	public void MultiplyF()
	{
		Assert.AreEqual(new Size(160, 80), new Size(200, 100).MultiplyF(.8f));
	}
}
