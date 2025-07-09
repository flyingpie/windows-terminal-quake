using Wtq.Services.KWin.ProcessFactory;

namespace Wtq.Services.KWin.UnitTest.ProcessFactory;

[TestClass]
public class NativeProcessFactoryTest
{
	private readonly WtqAppOptions _opts = new();
	private readonly NativeProcessFactory _pf = new();

	[TestMethod]
	public void FilenameMissing()
	{
		// Arrange
		_opts.FileName = null;

		// Act + Assert
		Assert.Throws<InvalidOperationException>(() => _pf.Create(_opts));
	}

	[TestMethod]
	public void Filename()
	{
		// Arrange
		_opts.FileName = "the-filename";

		// Act
		var proc = _pf.Create(_opts);

		// Assert
		AssertProcess.Equals(proc, "the-filename", string.Empty, []);
	}

	[TestMethod]
	public void Arguments()
	{
		// Arrange
		_opts.FileName = "the-filename";
		_opts.ArgumentList = [new("arg-1"), new("arg-2"), new("arg-3"),];

		// Act
		var proc = _pf.Create(_opts);

		// Assert
		AssertProcess.Equals(proc, "the-filename", string.Empty, ["arg-1", "arg-2", "arg-3"]);
	}

	[TestMethod]
	public void WorkingDir()
	{
		// Arrange
		_opts.FileName = "the-filename";
		_opts.WorkingDirectory = "/path/to/working/dir";

		// Act
		var proc = _pf.Create(_opts);

		// Assert
		AssertProcess.Equals(proc, "the-filename", "/path/to/working/dir", []);
	}

	[TestMethod]
	public void Full()
	{
		// Arrange
		_opts.FileName = "the-filename";
		_opts.ArgumentList = [new("arg-1"), new("arg-2"), new("arg-3"),];
		_opts.WorkingDirectory = "/path/to/working/dir";

		// Act
		var proc = _pf.Create(_opts);

		// Assert
		AssertProcess.Equals(proc, "the-filename", "/path/to/working/dir", ["arg-1", "arg-2", "arg-3"]);
	}
}