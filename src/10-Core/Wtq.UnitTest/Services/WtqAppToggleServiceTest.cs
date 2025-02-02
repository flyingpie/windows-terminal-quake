using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Wtq.Configuration;
using Wtq.Services;
using Wtq.Utils;

namespace Wtq.Core.UnitTest.Services;

[TestClass]
public class WtqAppToggleServiceTest
{
	private Mock<IOptionsMonitor<WtqOptions>> _wtqOptionsMock;
	private Mock<IWtqScreenInfoProvider> _scrInfoMock;
	private Mock<IWtqTween> _twnMock;

	private WtqAppToggleService _srv = null!;

	[TestInitialize]
	public void Setup()
	{
		_wtqOptionsMock = new(MockBehavior.Strict);
		_scrInfoMock = new(MockBehavior.Strict);
		_twnMock = new(MockBehavior.Strict);

		_srv = new WtqAppToggleService(_wtqOptionsMock.Object, _scrInfoMock.Object, _twnMock.Object);
	}

	[TestMethod]
	public async Task METHOD()
	{
		// _srv.ToggleOnAsync()
		Assert.Inconclusive();
	}
}