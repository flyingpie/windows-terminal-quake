namespace Wtq.Host.Linux.UnitTest;

[TestClass]
public class LinuxNativePlatformServiceTest
{
	private readonly LinuxNativePlatformService _p = new(
		pathToAppDir: "/path/to/app",
		pathToUserHomeDir: "/home/username");

	[TestMethod]
	public void PlatformName()
	{
		Assert.AreEqual("Linux Native", _p.PlatformName);
	}

	[TestMethod]
	public void DefaultApiUrls()
	{
		Assert.AreEqual(1, _p.DefaultApiUrls.Count);
		Assert.AreEqual("http://unix:/tmp/wtq.sock", _p.DefaultApiUrls.First());
	}

	[TestMethod]
	public void PathToAppDir()
	{
		Assert.AreEqual("/path/to/app", _p.PathToAppDir);
	}

	[TestMethod]
	public void PathToAppIcon()
	{
		Assert.AreEqual("/TODO", _p.PathToAppIcon);
	}

	[TestMethod]
	public void PathToAssetsDir()
	{
		Assert.AreEqual("/path/to/app/assets", _p.PathToAssetsDir);
	}

	[TestMethod]
	public void PathToLogsDir()
	{
		Assert.AreEqual("/TODO", _p.PathToLogsDir);
	}

	[TestMethod]
	public void PathToTempDir()
	{
		Assert.AreEqual("/TODO", _p.PathToTempDir);
	}

	[TestMethod]
	public void PathToTrayIcon()
	{
		Assert.AreEqual("/TODO", _p.PathToTrayIcon);
	}

	[TestMethod]
	public void PathToUserHomeDir()
	{
		Assert.AreEqual("/TODO", _p.PathToUserHomeDir);
	}
}