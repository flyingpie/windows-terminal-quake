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
		Assert.AreEqual(new Point(400, 250), new Size(800, 600).CenterInRectangle(new Rectangle(200, 100, 1200, 900)));
	}

	[TestMethod]
	public void Lerp()
	{
		Assert.AreEqual(new Point(350, 225), MathUtils.Lerp(new Point(200, 100), new Point(800, 600), .25f));
		Assert.AreEqual(new Point(500, 350), MathUtils.Lerp(new Point(200, 100), new Point(800, 600), .50f));
		Assert.AreEqual(new Point(650, 475), MathUtils.Lerp(new Point(200, 100), new Point(800, 600), .75f));
	}

	[TestMethod]
	public void MultiplyF()
	{
		Assert.AreEqual(new Size(160, 80), new Size(200, 100).MultiplyF(.8f));
	}
}