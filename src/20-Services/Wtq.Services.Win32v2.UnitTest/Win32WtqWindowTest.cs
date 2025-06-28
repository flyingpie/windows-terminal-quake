using Wtq.Configuration;
using Wtq.Utils;

namespace Wtq.Services.Win32v2.UnitTest;

[TestClass]
public class Win32WtqWindowTest
{
	private readonly Mock<IWin32> _win32 = new();
	private Win32Window _wnd = new(() => false);
	private WtqAppOptions _opts = new();

	private Win32WtqWindow _window = null!;

	[TestInitialize]
	public void Setup()
	{
		Log.Configure();

		_window = new Win32WtqWindow(_win32.Object, _wnd);
	}

	/// <summary>
	/// Match by process name, using the <see cref="WtqAppOptions.FileName"/> property.
	/// </summary>
	[TestMethod]
	[DataRow("ProcessExplorer", "ProcessExplorer", true)]
	[DataRow("ProcessExplorer", "processexplorer", true)]
	[DataRow("processexplorer", "ProcessExplorer", true)]
	public void ByProcessName_FromFileName_True(string opt, string wnd, bool isMatch)
	{
		// Arrange
		_opts.FileName = opt;
		_wnd.ProcessName = wnd;

		// Act + Assert
		Assert.AreEqual(isMatch, _window.Matches(_opts));
	}

	/// <summary>
	/// Match by process name, using the <see cref="WtqAppOptions.ProcessName"/> property.
	/// </summary>
	[TestMethod]

	//
	[DataRow("ProcessExplorer", "ProcessExplorer", true)]
	[DataRow("ProcessExplorer", "processexplorer", true)]
	[DataRow("processexplorer", "ProcessExplorer", true)]

	//
	[DataRow("procexp", "ProcessExplorer", false)]

	//
	[DataRow("ProcessExp", "ProcessExplorer", true)] // Without explicit regex symbols, acts as "contains".
	[DataRow("ProcessExp.*", "ProcessExplorer", true)]
	[DataRow(".*cessExp.*", "ProcessExplorer", true)]
	[DataRow(".rocessExplorer", "ProcessExplorer", true)]
	public void ByProcessName_FromProcessName_True(string opt, string wnd, bool isMatch)
	{
		// Arrange
		_opts.ProcessName = opt;
		_wnd.ProcessName = wnd;

		// Act + Assert
		Assert.AreEqual(isMatch, _window.Matches(_opts));
	}
}