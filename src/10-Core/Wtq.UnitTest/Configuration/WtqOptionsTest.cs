namespace Wtq.Core.UnitTest.Configuration;

[TestClass]
public class WtqOptionsTest
{
	private readonly WtqOptions _glbl = new();
	private readonly WtqAppOptions _app = new();

	[TestMethod]
	public void Cascades_Default()
	{
		var res = _glbl.GetAnimationDurationMs(_app);

		Assert.AreEqual(250, res);
	}

	[TestMethod]
	public void Cascades_Global()
	{
		_glbl.AnimationDurationMs = 1234;

		var res = _glbl.GetAnimationDurationMs(_app);

		Assert.AreEqual(1234, res);
	}

	[TestMethod]
	public void Cascades_App()
	{
		_glbl.AnimationDurationMs = 1234;
		_app.AnimationDurationMs = 4321;

		var res = _glbl.GetAnimationDurationMs(_app);

		Assert.AreEqual(4321, res);
	}
}