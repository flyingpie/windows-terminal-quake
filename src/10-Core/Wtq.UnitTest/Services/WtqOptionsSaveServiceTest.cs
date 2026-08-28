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

		Assert.Inconclusive("TODO");
	}

	[TestMethod]
	public void FeatureFlags_SharpHookOff_IsWritten()
	{
		// Arrange
		_opts.FeatureFlags = new() { SharpHook = false };

		// Act
		var act = _svc.Write(_opts);

		// Assert
		StringAssert.Contains(act, "\"FeatureFlags\"");
		StringAssert.Contains(act, "\"SharpHook\": false");
	}

	[TestMethod]
	public void FeatureFlags_SharpHookOn_IsWritten()
	{
		// Arrange
		_opts.FeatureFlags = new() { SharpHook = true };

		// Act
		var act = _svc.Write(_opts);

		// Assert
		StringAssert.Contains(act, "\"SharpHook\": true");
	}

	[TestMethod]
	public void FeatureFlags_NothingSet_IsOmitted()
	{
		// Arrange (e.g. the GUI always instantiates the object, so its settings can be bound)
		_opts.FeatureFlags = new();

		// Act
		var act = _svc.Write(_opts);

		// Assert
		Assert.IsFalse(act.Contains("FeatureFlags", StringComparison.Ordinal), $"Expected no 'FeatureFlags' in:\n{act}");
	}
}