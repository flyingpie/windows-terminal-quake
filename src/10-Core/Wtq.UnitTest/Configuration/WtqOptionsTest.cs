namespace Wtq.Core.UnitTest.Configuration;

[TestClass]
public class WtqOptionsTest
{
	private WtqOptions _glbl = null!;
	private WtqAppOptions _app = null!;

	[TestInitialize]
	public void Setup()
	{
		_glbl = new();
		_app = new();

		_app.Global = _glbl;
	}

	[TestMethod]
	public void Cascades_Default()
	{
		var res = _app.GetAnimationDurationMs();

		Assert.AreEqual(250, res);
	}

	[TestMethod]
	public void Cascades_Global()
	{
		_glbl.AnimationDurationMs = 1234;

		var res = _app.GetAnimationDurationMs();

		Assert.AreEqual(1234, res);
	}

	[TestMethod]
	public void Cascades_App()
	{
		_glbl.AnimationDurationMs = 1234;
		_app.AnimationDurationMs = 4321;

		var res = _app.GetAnimationDurationMs();

		Assert.AreEqual(4321, res);
	}
}