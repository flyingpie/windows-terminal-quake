using System;
using System.Linq;
using Wtq.Configuration;

namespace Wtq.Services.Win32v2.UnitTest;

[TestClass]
public class WindowsPlatformServiceTest
{
	private readonly WtqAppOptions _opts = new();

	private readonly WindowsPlatformService _p = new(
		pathToAppDir: "C:/path/to/app",
		pathToUserHomeDir: "C:/users/username");

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

	// [TestMethod]
	// public void PathToAppIcon()
	// {
	// 	Assert.AreEqual("/TODO", _p.PathToAppIcon);
	// }

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

	[TestMethod]
	public void PathToWtqConf()
	{
		Assert.AreEqual("/TODO", _p.PathToWtqConf);
	}

	[TestMethod]
	public void PathToWtqConfDir()
	{
		Assert.AreEqual("/TODO", _p.PathToWtqConfDir);
	}

	[TestMethod]
	public void PathsToWtqConfs()
	{
		Assert.AreEqual(1234, _p.PathsToWtqConfs.Count);
	}

	[TestMethod]
	public void PreferredPathWtqConfig()
	{
		Assert.AreEqual("/TODO", _p.PreferredPathWtqConfig);
	}

	#region CreateProcess

	[TestMethod]
	public void CreateProcess_FilenameMissing()
	{
		// Arrange
		_opts.FileName = null;

		// Act + Assert
		Assert.Throws<InvalidOperationException>(() => _p.CreateProcess(_opts));
	}

	[TestMethod]
	public void CreateProcess_Filename()
	{
		// Arrange
		_opts.FileName = "the-filename";

		// Act
		var proc = _p.CreateProcess(_opts);

		// Assert
		AssertProcess.Equals(proc, "the-filename", string.Empty, []);
	}

	[TestMethod]
	public void CreateProcess_Arguments()
	{
		// Arrange
		_opts.FileName = "the-filename";
		_opts.ArgumentList = [new("arg-1"), new("arg-2"), new("arg-3"),];

		// Act
		var proc = _p.CreateProcess(_opts);

		// Assert
		AssertProcess.Equals(proc, "the-filename", string.Empty, ["arg-1", "arg-2", "arg-3"]);
	}

	[TestMethod]
	public void CreateProcess_WorkingDir()
	{
		// Arrange
		_opts.FileName = "the-filename";
		_opts.WorkingDirectory = "/path/to/working/dir";

		// Act
		var proc = _p.CreateProcess(_opts);

		// Assert
		AssertProcess.Equals(proc, "the-filename", "/path/to/working/dir", []);
	}

	[TestMethod]
	public void CreateProcess_Full()
	{
		// Arrange
		_opts.FileName = "the-filename";
		_opts.ArgumentList = [new("arg-1"), new("arg-2"), new("arg-3"),];
		_opts.WorkingDirectory = "/path/to/working/dir";

		// Act
		var proc = _p.CreateProcess(_opts);

		// Assert
		AssertProcess.Equals(proc, "the-filename", "/path/to/working/dir", ["arg-1", "arg-2", "arg-3"]);
	}

	#endregion
}