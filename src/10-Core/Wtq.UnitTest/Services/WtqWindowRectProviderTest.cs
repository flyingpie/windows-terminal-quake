using System.Threading.Tasks;
using static Wtq.Configuration.HorizontalAlign;
using static Wtq.Configuration.Resizing;

namespace Wtq.Core.UnitTest.Services;

[TestClass]
public class WtqWindowRectProviderTest
{
	private readonly Mock<IWtqScreenInfoProvider> _screenInfoProvider = new(MockBehavior.Strict);
	private readonly WtqAppOptions _opts = new();

	private WtqWindowRectProvider _wndRectProvider = null!;

	[TestInitialize]
	public void Setup()
	{
		_wndRectProvider = new WtqWindowRectProvider(_screenInfoProvider.Object);
	}

	[TestMethod]
	// v-align
	//			h-align,	h-cov,	v-cov,	v-offset,	resize,		scr[]										wnd[]
	[DataRow(	Center,		50,		50,		0,			Always,		0,		0,		1920,		1080,			480,	0,		960,	540)]
	// Left - Center - Right
	[DataRow(	Left,		50,		50,		0,			Always,		0,		0,		1920,		1080,			0,		0,		960,	540)] // Left
	[DataRow(	Center,		50,		50,		0,			Always,		0,		0,		1920,		1080,			480,	0,		960,	540)] // Center
	[DataRow(	Right,		50,		50,		0,			Always,		0,		0,		1920,		1080,			960,	0,		960,	540)] // Right
	// Vertical offset
	[DataRow(	Center,		50,		50,		0,			Always,		0,		0,		1920,		1080,			480,	0,		960,	540)] // 0
	[DataRow(	Center,		50,		50,		50,			Always,		0,		0,		1920,		1080,			480,	50,		960,	540)] // 50
	[DataRow(	Center,		50,		50,		150,		Always,		0,		0,		1920,		1080,			480,	150,	960,	540)] // 150
	// Resize
	[DataRow(	Center,		50,		50,		0,			Always,		0,		0,		1920,		1080,			480,	0,		960,	540)] // True
	[DataRow(	Center,		50,		50,		0,			Never,		0,		0,		1920,		1080,			560,	0,		800,	600)] // False
	public async Task Test1(
		HorizontalAlign hAlign, int hCov, int vCov, int vOffs, Resizing resize,		// Alignment
		int sX, int sY, int sW, int sH,												// Screen
		int wX, int wY, int wW, int wH												// Expected window
	)
	{
		var currWindowRect = new Rectangle(0, 0, 800, 600);

		_opts.HorizontalAlign = hAlign;
		_opts.HorizontalScreenCoverage = hCov;
		_opts.VerticalScreenCoverage = vCov;
		_opts.VerticalOffset = vOffs;
		_opts.Resize = resize;

		var res = await _wndRectProvider.GetOnScreenRectAsync(
			screenRectSrc: new(sX, sY, sW, sH),
			currWindowRect,
			opts: _opts
		);

		Assert.AreEqual(wX, res.X);
		Assert.AreEqual(wY, res.Y);
		Assert.AreEqual(wW, res.Width);
		Assert.AreEqual(wH, res.Height);
	}
}

[TestClass]
public class WtqTargetScreenRectProviderTest
{
	private readonly Mock<IWtqScreenInfoProvider> _screenInfoProvider = new(MockBehavior.Strict);

	private WtqTargetScreenRectProvider _provider = null!;

	[TestInitialize]
	public void Setup()
	{
		_provider = new(_screenInfoProvider.Object);
	}

	[TestMethod]
	public async Task Test1()
	{

	}
}