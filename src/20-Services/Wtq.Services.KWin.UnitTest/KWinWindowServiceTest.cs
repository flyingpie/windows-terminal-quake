using System.Collections.ObjectModel;
using System.Diagnostics;
using Wtq.Configuration;
using Wtq.Utils;

namespace Wtq.Services.KWin.UnitTest;

[TestClass]
public class KWinWindowServiceTest
{
	private readonly Mock<IKWinClient> _kwin = new();
	private readonly WtqAppOptions _opts = new();

	private KWinWindowService _win;

	[TestInitialize]
	public void Setup()
	{
		Log.Configure();

		_win = new(_kwin.Object);
	}

	[TestMethod]
	public void CreateProcessStartInfo_Filename()
	{
		// Arrange
		_opts.FileName = "the-filename";

		// Act
		var startInfo = _win.CreateProcessStartInfo(_opts);

		// Assert
		AssertStartInfo(startInfo, "the-filename", string.Empty, []);
	}

	[TestMethod]
	public void CreateProcessStartInfo_Arguments()
	{
		// Arrange
		_opts.FileName = "the-filename";
		_opts.ArgumentList = [new("arg-1"), new("arg-2"), new("arg-3"),];

		// Act
		var startInfo = _win.CreateProcessStartInfo(_opts);

		// Assert
		AssertStartInfo(startInfo, "the-filename", string.Empty, ["arg-1", "arg-2", "arg-3"]);
	}

	[TestMethod]
	public void CreateProcessStartInfo_Full()
	{
		// Arrange
		_opts.FileName = "the-filename";
		_opts.ArgumentList = [new("arg-1"), new("arg-2"), new("arg-3"),];
		_opts.WorkingDirectory = "/path/to/working/dir";

		// Act
		var startInfo = _win.CreateProcessStartInfo(_opts);

		// Assert
		AssertStartInfo(startInfo, "the-filename", "/path/to/working/dir", ["arg-1", "arg-2", "arg-3"]);
	}

	[TestMethod]
	public void CreateProcessStartInfoFlatpak_Filename()
	{
		// Arrange
		_opts.FileName = "the-filename";

		// Act
		var startInfo = _win.CreateProcessStartInfoFlatpak(_opts);

		// Assert
		AssertStartInfo(startInfo, "flatpak-spawn", string.Empty, ["--host", "the-filename"]);
	}

	[TestMethod]
	public void CreateProcessStartInfoFlatpak_Full()
	{
		// Arrange
		_opts.FileName = "the-filename";
		_opts.ArgumentList = [new("arg-1"), new("arg-2"), new("arg-3"),];
		_opts.WorkingDirectory = "/path/to/working/dir";

		// Act
		var startInfo = _win.CreateProcessStartInfoFlatpak(_opts);

		// Assert
		AssertStartInfo(startInfo, "flatpak-spawn", string.Empty, ["--host", "--directory", "/path/to/working/dir", "the-filename", "arg-1", "arg-2", "arg-3"]);
	}

	private static void AssertStartInfo(
		ProcessStartInfo startInfo,
		string fileName,
		string workingDir,
		Collection<string> arguments)
	{
		// Filename
		Assert.AreEqual(fileName, startInfo.FileName);

		// Working dir
		Assert.AreEqual(workingDir, startInfo.WorkingDirectory);

		// Arguments
		Assert.IsNotNull(startInfo.ArgumentList);
		Assert.AreEqual(arguments.Count, startInfo.ArgumentList.Count);
		for (var i = 0; i < arguments.Count; i++)
		{
			Assert.AreEqual(arguments[i], startInfo.ArgumentList[i]);
		}
	}
}