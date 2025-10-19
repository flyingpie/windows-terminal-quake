using Wtq.Core.UnitTest.Utils;

namespace Wtq.Core.UnitTest.Services;

[TestClass]
public class WtqOptionsSaveServiceTest
{
	private WtqOptions _opts = new();

	private WtqOptionsSaveService _svc = new(new Mock<IPlatformService>(MockBehavior.Strict).Object);

	[TestMethod]
	public void Empty()
	{
		// Act
		var act = _svc.Write(new());

		// Assert
		var exp =
			"""
			{
				"$schema": "wtq.schema.json"
			}
			""";

		Assert.That.JsonAreEqual(exp, act);
	}
}