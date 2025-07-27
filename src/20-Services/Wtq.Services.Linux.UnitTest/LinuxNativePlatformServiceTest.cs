using Wtq.Utils;

namespace Wtq.Services.Linux.UnitTest;

[TestClass]
public class LinuxNativePlatformServiceTest
{
	private readonly Mock<IFs> _fs = new(MockBehavior.Strict);
	private readonly WtqAppOptions _opts = new();

	private readonly LinuxNativePlatformService _p = new(
		pathToAppDir: "/path/to/app",
		pathToUserHomeDir: "/home/username");

	[TestInitialize]
	public void Setup()
	{
		Environment.SetEnvironmentVariable("WTQ_CONFIG_FILE", "");
		Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", "");
		Environment.SetEnvironmentVariable("XDG_STATE_HOME", "");
		Environment.SetEnvironmentVariable("XDG_RUNTIME_DIR", "");

		Fs.Inst = _fs.Object;

		_fs.Reset();
	}

	[TestMethod]
	public void PlatformName()
	{
		Assert.AreEqual("Linux", _p.PlatformName);
	}

	[TestMethod]
	public void DefaultApiUrls_XdgRuntimeNotSet()
	{
		Assert.AreEqual(1, _p.DefaultApiUrls.Count);
		Assert.AreEqual("http://unix:/home/username/.local/state/wtq/wtq.sock", _p.DefaultApiUrls.First());
	}

	[TestMethod]
	public void DefaultApiUrls_XdgRuntimeSet()
	{
		Environment.SetEnvironmentVariable("XDG_RUNTIME_DIR", "/path/to/runtime/dir");

		Assert.AreEqual(1, _p.DefaultApiUrls.Count);
		Assert.AreEqual("http://unix:/path/to/runtime/dir/wtq/wtq.sock", _p.DefaultApiUrls.First());
	}

	[TestMethod]
	public void PathToAppDir()
	{
		Assert.AreEqual("/path/to/app", _p.PathToAppDir);
	}

	[TestMethod]
	public void PathToAssetsDir()
	{
		Assert.AreEqual("/path/to/app/assets", _p.PathToAssetsDir);
	}

	[TestMethod]
	public void PathToLogsDir_XdgNotSet()
	{
		Assert.AreEqual("/home/username/.local/state/wtq", _p.PathToLogsDir);
	}

	[TestMethod]
	public void PathToLogsDir_XdgSet()
	{
		Environment.SetEnvironmentVariable("XDG_STATE_HOME", "/path/to/state/home");

		Assert.AreEqual("/path/to/state/home/wtq", _p.PathToLogsDir);
	}

	[TestMethod]
	public void PathToTempDir_XdgNotSet()
	{
		Assert.AreEqual("/home/username/.local/state/wtq", _p.PathToTempDir);
	}

	[TestMethod]
	public void PathToTempDir_XdgSet()
	{
		Environment.SetEnvironmentVariable("XDG_STATE_HOME", "/path/to/state/home");

		Assert.AreEqual("/path/to/state/home/wtq", _p.PathToTempDir);
	}

	[TestMethod]
	public void PathToTrayIconDark()
	{
		_fs.Setup(m => m.FileExists(It.IsAny<string>())).Returns(true);

		Assert.AreEqual("/path/to/app/assets/nl.flyingpie.wtq-black.svg", _p.PathToTrayIconDark);
	}

	[TestMethod]
	public void PathToTrayIconLight()
	{
		_fs.Setup(m => m.FileExists(It.IsAny<string>())).Returns(true);

		Assert.AreEqual("/path/to/app/assets/nl.flyingpie.wtq-white.svg", _p.PathToTrayIconLight);
	}

	[TestMethod]
	public void PathToUserHomeDir()
	{
		Assert.AreEqual("/home/username", _p.PathToUserHomeDir);
	}

	[TestMethod]
	[DataRow("/path/to/app/wtq.json")]
	[DataRow("/home/username/wtq.json")]
	public void PathToWtqConf(string path)
	{
		_fs.Reset();
		_fs.Setup(m => m.FileExists(It.IsAny<string>())).Returns(false);
		_fs.Setup(m => m.FileExists(It.Is<string>(p => p == path))).Returns(true);

		Assert.AreEqual(path, _p.PathToWtqConf);
	}

	[TestMethod]
	public void PathToWtqConf_Preferred()
	{
		_fs.Setup(m => m.FileExists(It.IsAny<string>())).Returns(false);

		Assert.AreEqual("/home/username/.config/wtq.jsonc", _p.PathToWtqConf);
	}

	[TestMethod]
	[DataRow("/path/to/app/wtq.json", "/path/to/app")]
	[DataRow("/home/username/wtq.json", "/home/username")]
	public void PathToWtqConfDir(string path, string dir)
	{
		_fs.Reset();
		_fs.Setup(m => m.FileExists(It.IsAny<string>())).Returns(false);
		_fs.Setup(m => m.FileExists(It.Is<string>(p => p == path))).Returns(true);

		Assert.AreEqual(dir, _p.PathToWtqConfDir);
	}

	[TestMethod]
	public void PathToWtqConfDir_Preferred()
	{
		_fs.Setup(m => m.FileExists(It.IsAny<string>())).Returns(false);

		Assert.AreEqual("/home/username/.config", _p.PathToWtqConfDir);
	}

	[TestMethod]
	public void PathsToWtqConfs_WtqConfigFileEnvSet()
	{
		Environment.SetEnvironmentVariable("WTQ_CONFIG_FILE", "/env/path/to/wtq.jsonc");

		var paths = _p.PathsToWtqConfs.ToList();

		Assert.AreEqual(19, paths.Count);
		Assert.AreEqual("/env/path/to/wtq.jsonc", paths[0]);

		// Other stuff is tested in the other tests.
	}

	[TestMethod]
	public void PathsToWtqConfs_XdgSet()
	{
		Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", "/path/to/xdg/config/home");

		var paths = _p.PathsToWtqConfs.ToList();

		var expected = new[]
		{
			// Next to wtq executable.
			"/path/to/app/wtq.json",
			"/path/to/app/wtq.jsonc",
			"/path/to/app/wtq.json5",
			"/path/to/app/.wtq.json",
			"/path/to/app/.wtq.jsonc",
			"/path/to/app/.wtq.json5",

			// In XDG config dir.
			"/path/to/xdg/config/home/wtq.json",
			"/path/to/xdg/config/home/wtq.jsonc",
			"/path/to/xdg/config/home/wtq.json5",
			"/path/to/xdg/config/home/.wtq.json",
			"/path/to/xdg/config/home/.wtq.jsonc",
			"/path/to/xdg/config/home/.wtq.json5",

			// In user home dir.
			"/home/username/wtq.json",
			"/home/username/wtq.jsonc",
			"/home/username/wtq.json5",
			"/home/username/.wtq.json",
			"/home/username/.wtq.jsonc",
			"/home/username/.wtq.json5",
		};

		Assert.AreEqual(expected.Length, paths.Count);

		for (var i = 0; i < paths.Count; i++)
		{
			Assert.AreEqual(expected[i], paths[i]);
		}
	}

	[TestMethod]
	public void PathsToWtqConfs_XdgNotSet()
	{
		var paths = _p.PathsToWtqConfs.ToList();

		var expected = new[]
		{
			// Next to wtq executable.
			"/path/to/app/wtq.json",
			"/path/to/app/wtq.jsonc",
			"/path/to/app/wtq.json5",
			"/path/to/app/.wtq.json",
			"/path/to/app/.wtq.jsonc",
			"/path/to/app/.wtq.json5",

			// In XDG config dir.
			"/home/username/.config/wtq.json",
			"/home/username/.config/wtq.jsonc",
			"/home/username/.config/wtq.json5",
			"/home/username/.config/.wtq.json",
			"/home/username/.config/.wtq.jsonc",
			"/home/username/.config/.wtq.json5",

			// In user home dir.
			"/home/username/wtq.json",
			"/home/username/wtq.jsonc",
			"/home/username/wtq.json5",
			"/home/username/.wtq.json",
			"/home/username/.wtq.jsonc",
			"/home/username/.wtq.json5",
		};

		Assert.AreEqual(expected.Length, paths.Count);

		for (var i = 0; i < paths.Count; i++)
		{
			Assert.AreEqual(expected[i], paths[i]);
		}
	}

	[TestMethod]
	public void PreferredPathWtqConfig_WtqConfigFileEnvSet()
	{
		Environment.SetEnvironmentVariable("WTQ_CONFIG_FILE", "/env/path/to/wtq.jsonc");
		Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", "/path/to/xdg/config/home");

		Assert.AreEqual("/env/path/to/wtq.jsonc", _p.PreferredPathWtqConfig);
	}

	[TestMethod]
	public void PreferredPathWtqConfig_XdgSet()
	{
		Environment.SetEnvironmentVariable("WTQ_CONFIG_FILE", "");
		Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", "/path/to/xdg/config/home");

		Assert.AreEqual("/path/to/xdg/config/home/wtq.jsonc", _p.PreferredPathWtqConfig);
	}

	[TestMethod]
	public void PreferredPathWtqConfig_XdgNotSet()
	{
		Environment.SetEnvironmentVariable("WTQ_CONFIG_FILE", "");
		Environment.SetEnvironmentVariable("XDG_CONFIG_HOME", "");

		Assert.AreEqual("/home/username/.config/wtq.jsonc", _p.PreferredPathWtqConfig);
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