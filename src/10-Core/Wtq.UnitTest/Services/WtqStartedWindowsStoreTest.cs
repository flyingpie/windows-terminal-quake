using System.IO;

namespace Wtq.Core.UnitTest.Services;

[TestClass]
public class WtqStartedWindowsStoreTest
{
	private readonly Mock<IPlatformService> _platform = new(MockBehavior.Strict);

	private string _dir = null!;

	[TestInitialize]
	public void Setup()
	{
		_dir = Path.Combine(Path.GetTempPath(), "wtq-test-" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(_dir);

		_platform.SetupGet(p => p.PathToWtqConfDir).Returns(_dir);
	}

	[TestCleanup]
	public void Cleanup()
	{
		Directory.Delete(_dir, recursive: true);
	}

	[TestMethod]
	public void NoStateFile_ReturnsNull()
	{
		var store = new WtqStartedWindowsStore(_platform.Object);

		Assert.IsNull(store.GetWindowId("App"));
	}

	[TestMethod]
	public void SetAndRemove_PersistAcrossInstances()
	{
		// Set
		new WtqStartedWindowsStore(_platform.Object).SetWindowId("App", "1234");

		// Read through a fresh instance (simulates a WTQ restart).
		var store2 = new WtqStartedWindowsStore(_platform.Object);
		Assert.AreEqual("1234", store2.GetWindowId("App"));
		Assert.AreEqual("1234", store2.GetWindowId("app"), "App names should be case-insensitive.");
		Assert.IsNull(store2.GetWindowId("Other"));

		// Remove
		store2.RemoveWindowId("App");

		Assert.IsNull(new WtqStartedWindowsStore(_platform.Object).GetWindowId("App"));
	}

	[TestMethod]
	public void CorruptStateFile_StartsEmpty()
	{
		File.WriteAllText(Path.Combine(_dir, "wtq.state.json"), "this is not json");

		var store = new WtqStartedWindowsStore(_platform.Object);

		Assert.IsNull(store.GetWindowId("App"));

		// And can still be written to.
		store.SetWindowId("App", "1");
		Assert.AreEqual("1", new WtqStartedWindowsStore(_platform.Object).GetWindowId("App"));
	}
}
