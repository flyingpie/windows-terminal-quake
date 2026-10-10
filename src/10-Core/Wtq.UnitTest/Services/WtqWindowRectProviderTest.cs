using HA = Wtq.Configuration.HorizontalAlign;
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

	// csharpier-ignore-start
	[TestMethod]
	//				h-align,		h-cov,	v-cov,	v-offset,	resize,			screen(x,y,w,h)								cur window(x,y,w,h)					exp window(x,y,w,h)
	[DataRow(01,	HA.Center,		50,		50,		0,			Always,			0,		0,		1920,		1080,			0,		0,		0,		0,			480,	0,		960,	540)]
	// Left - Center - Right																																								//
	[DataRow(02,	HA.Left,		50,		50,		0,			Always,			0,		0,		1920,		1080,			0,		0,		0,		0,			0,		0,		960,	540)]	// Left
	[DataRow(03,	HA.Center,		50,		50,		0,			Always,			0,		0,		1920,		1080,			0,		0,		0,		0,			480,	0,		960,	540)]	// Center
	[DataRow(04,	HA.Right,		50,		50,		0,			Always,			0,		0,		1920,		1080,			0,		0,		0,		0,			960,	0,		960,	540)]	// Right
	// Vertical offset																																										//
	[DataRow(05,	HA.Center,		50,		50,		0,			Always,			0,		0,		1920,		1080,			0,		0,		0,		0,			480,	0,		960,	540)]	// 0
	[DataRow(06,	HA.Center,		50,		50,		50,			Always,			0,		0,		1920,		1080,			0,		0,		0,		0,			480,	50,		960,	540)]	// 50
	[DataRow(07,	HA.Center,		50,		50,		150,		Always,			0,		0,		1920,		1080,			0,		0,		0,		0,			480,	150,	960,	540)]	// 150
	// Resize																																												//
	[DataRow(08,	HA.Center,		50,		50,		0,			Always,			0,		0,		1920,		1080,			0,		0,		0,		0,			480,	0,		960,	540)]	// True
	[DataRow(09,	HA.Center,		50,		50,		0,			Never,			0,		0,		1920,		1080,			0,		0,		800,	600,		560,	0,		800,	600)]	// False
	// HorizontalAlign=None																																									//
	[DataRow(10,	HA.None,		0,		0,		0,			Never,			0,		0,		1920,		1080,			123,	0,		800,	600,		123,	0,		800,	600)]	// Top-Left
	// csharpier-ignore-end
	public async Task GetOnScreenRectAsyncTest(
		int id,																		// Id, to make it easier to identify in the test runner
		HorizontalAlign hAlign, int hCov, int vCov, int vOffs, Resizing resize,		// Alignment
		int scrX, int scrY, int scrW, int scrH,										// Screen
		int curWinX, int curWinY, int curWinW, int curWinH,							// Current window
		int expWinX, int expWinY, int expWinW, int expWinH							// Expected window
	)
	{
		_opts.HorizontalAlign = hAlign;
		_opts.HorizontalScreenCoverage = hCov;
		_opts.VerticalScreenCoverage = vCov;
		_opts.VerticalOffset = vOffs;
		_opts.Resize = resize;

		var res = await _wndRectProvider.GetOnScreenRectAsync(
			screenRectDst: new(scrX, scrY, scrW, scrH),
			windowRectSrc: new(curWinX, curWinY, curWinW, curWinH),
			opts: _opts
		);

		Assert.AreEqual(expWinX, res.X);
		Assert.AreEqual(expWinY, res.Y);
		Assert.AreEqual(expWinW, res.Width);
		Assert.AreEqual(expWinH, res.Height);
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