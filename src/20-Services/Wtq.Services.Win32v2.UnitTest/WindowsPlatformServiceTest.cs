using System;
using System.Linq;
using Wtq.Configuration;
using Wtq.Utils;

namespace Wtq.Services.Win32v2.UnitTest;

[TestClass]
public class WindowsPlatformServiceTest
{
	private readonly Mock<IFs> _fs = new(MockBehavior.Strict);
	private readonly WtqAppOptions _opts = new();

	private readonly WindowsPlatformService _p = new(
		pathToAppDir: "C:/path/to/app",
		pathToUserHomeDir: "C:/users/username",
		pathToTempDir: "C:/path/to/tmp");

	[TestInitialize]
	public void Setup()
	{
		Environment.SetEnvironmentVariable("WTQ_CONFIG_FILE", "");

		Fs.Inst = _fs.Object;

		_fs.Reset();
	}

	[TestMethod]
	public void PlatformName()
	{
		Assert.AreEqual("Windows", _p.PlatformName);
	}

	[TestMethod]
	public void DefaultApiUrls()
	{
		Assert.AreEqual(1, _p.DefaultApiUrls.Count);
		Assert.AreEqual("http://pipe:/wtq", _p.DefaultApiUrls.First());
	}

	[TestMethod]
	public void PathToAppDir()
	{
		Assert.AreEqual("C:/path/to/app", _p.PathToAppDir);
	}

	[TestMethod]
	public void PathToAssetsDir()
	{
		Assert.AreEqual("C:/path/to/app/assets", _p.PathToAssetsDir);
	}

	[TestMethod]
	public void PathToLogsDir()
	{
		_fs.Setup(m => m.DirExists("C:/path/to/tmp/wtq")).Returns(true);

		Assert.AreEqual("C:/path/to/tmp/wtq", _p.PathToLogsDir);
	}

	[TestMethod]
	public void PathToTempDir()
	{
		_fs.Setup(m => m.DirExists("C:/path/to/tmp/wtq")).Returns(true);

		Assert.AreEqual("C:/path/to/tmp/wtq", _p.PathToTempDir);
	}

	[TestMethod]
	public void PathToTrayIconDark()
	{
		_fs.Setup(m => m.FileExists(It.IsAny<string>())).Returns(true);

		Assert.AreEqual("C:/path/to/app/assets/nl.flyingpie.wtq-black.ico", _p.PathToTrayIconDark);
	}

	[TestMethod]
	public void PathToTrayIconLight()
	{
		_fs.Setup(m => m.FileExists(It.IsAny<string>())).Returns(true);

		Assert.AreEqual("C:/path/to/app/assets/nl.flyingpie.wtq-white.ico", _p.PathToTrayIconLight);
	}

	[TestMethod]
	public void PathToUserHomeDir()
	{
		Assert.AreEqual("C:/users/username", _p.PathToUserHomeDir);
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
		Assert.AreEqual("C:/users/username/AppData/Roaming/wtq/wtq.jsonc", _p.PreferredPathWtqConfig);
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