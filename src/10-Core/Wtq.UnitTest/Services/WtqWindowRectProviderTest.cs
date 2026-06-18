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
	public async Task GetOnScreenRectAsyncTest(
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
			screenRectDst: new(sX, sY, sW, sH),
			currWindowRect,
			opts: _opts
		);

		Assert.AreEqual(wX, res.X);
		Assert.AreEqual(wY, res.Y);
		Assert.AreEqual(wW, res.Width);
		Assert.AreEqual(wH, res.Height);
	}

	[TestMethod]
	// Secondary screen vertical coverage: when isPrimaryScreen is false,
	// VerticalScreenCoverageSecondScreen should be used instead of VerticalScreenCoverage.
	//			h-cov,	vCovPrim,	vCovSec,	vOffs,				scr[],										wnd[]
	[DataRow(	50,		80,			100,		0,		0,		0,		1920,	1080,			480,	0,		960,	1080)]	// Sec 100%: full height on secondary
	[DataRow(	50,		80,			50,			0,		0,		0,		1920,	1080,			480,	0,		960,	540)]	// Sec 50%: half height on secondary
	public async Task GetOnScreenRectAsync_SecondScreen_UsesSecondaryCoverage(
		int hCov, int vCovPrim, int vCovSec, int vOffs,
		int sX, int sY, int sW, int sH,
		int wX, int wY, int wW, int wH
	)
	{
		var currWindowRect = new Rectangle(0, 0, 800, 600);

		_opts.HorizontalAlign = HorizontalAlign.Center;
		_opts.HorizontalScreenCoverage = hCov;
		_opts.VerticalScreenCoverage = vCovPrim;
		_opts.VerticalScreenCoverageSecondScreen = vCovSec;
		_opts.VerticalOffset = vOffs;
		_opts.Resize = Resizing.Always;

		var res = await _wndRectProvider.GetOnScreenRectAsync(
			screenRectDst: new(sX, sY, sW, sH),
			currWindowRect,
			opts: _opts,
			isPrimaryScreen: false
		);

		Assert.AreEqual(wX, res.X);
		Assert.AreEqual(wY, res.Y);
		Assert.AreEqual(wW, res.Width);
		Assert.AreEqual(wH, res.Height);
	}

	[TestMethod]
	public async Task GetOnScreenRectAsync_SecondScreen_FallsBackToPrimaryCoverage_WhenNotSet()
	{
		var currWindowRect = new Rectangle(0, 0, 800, 600);

		// Set primary coverage to 80%, leave secondary coverage unset (null).
		// When VerticalScreenCoverageSecondScreen is null, it should fall back to VerticalScreenCoverage (80%).
		var globalOpts = new WtqOptions
		{
			HorizontalAlign = HorizontalAlign.Center,
			HorizontalScreenCoverage = 50,
			VerticalScreenCoverage = 80,
		};

		_opts.Global = globalOpts;
		_opts.HorizontalScreenCoverage = 50;
		_opts.VerticalScreenCoverage = 80;
		_opts.VerticalScreenCoverageSecondScreen = null;
		_opts.Resize = Resizing.Always;

		var res = await _wndRectProvider.GetOnScreenRectAsync(
			screenRectDst: new(1920, 0, 1920, 1080),
			currWindowRect,
			opts: _opts,
			isPrimaryScreen: false
		);

		// Height should be 80% of 1080 = 864, not 100% (no DefaultValue) or 50% (some other value).
		Assert.AreEqual(864, res.Height, "Should fall back to primary VerticalScreenCoverage when secondary is not set");
		// X should be 1920 + (1920 * 50 / 100) / 2 = 2400 (centered on secondary screen)
		Assert.AreEqual(2400, res.X);
		Assert.AreEqual(960, res.Width);
	}

	[TestMethod]
	//			off-screen-locs,											scr[]										wnd[]
	[DataRow(	new[] { OffScreenLocation.Above },							0,		0,		1920,		1080,			0,		-700,	800,	600)]
	public async Task GetOffScreenRectAsyncTest(
		OffScreenLocation[] locs,
		int sX, int sY, int sW, int sH,												// Screen
		int wX, int wY, int wW, int wH												// Expected window
	)
	{
		var currWindowRect = new Rectangle(0, 0, 800, 600);

		_opts.OffScreenLocations = locs;

		_screenInfoProvider.Setup(m => m.GetScreenRectsAsync()).ReturnsAsync([new(sX, sY, sW, sH)]);

		var res = await _wndRectProvider.GetOffScreenRectAsync(
			screenRectSrc: new(sX, sY, sW, sH),
			currWindowRect,
			opts: _opts
		);

		Assert.AreEqual(wX, res.Value.X);
		Assert.AreEqual(wY, res.Value.Y);
		Assert.AreEqual(wW, res.Value.Width);
		Assert.AreEqual(wH, res.Value.Height);
	}
}