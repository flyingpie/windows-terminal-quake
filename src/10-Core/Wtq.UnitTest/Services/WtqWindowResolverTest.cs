using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Wtq.Core.UnitTest.Services;

[TestClass]
public class WtqWindowResolverTest
{
	private readonly Mock<IWtqWindowService> _windowService = new(MockBehavior.Strict);
	private readonly Mock<IWtqStartedWindowsStore> _startedWindows = new(MockBehavior.Loose);

	private readonly WtqWindow _preExisting = CreateWindow("pre-existing");
	private readonly WtqWindow _started = CreateWindow("started");

	private WtqWindowResolver _resolver = null!;

	[TestInitialize]
	public void Setup()
	{
		_resolver = new WtqWindowResolver(_windowService.Object, _startedWindows.Object);
	}

	[TestMethod]
	public async Task StartOnly_NotAllowedToStart_DoesNotAttachToPreExistingWindow()
	{
		// Arrange
		var opts = CreateOpts(AttachMode.StartOnly);

		_windowService
			.Setup(s => s.FindWindowsAsync(opts, It.IsAny<CancellationToken>()))
			.ReturnsAsync([_preExisting]);

		// Act
		var window = await _resolver.GetWindowHandleAsync(opts, allowStartNew: false);

		// Assert
		Assert.IsNull(window, "Should not attach to a window that WTQ did not start.");

		_windowService.Verify(s => s.CreateAsync(It.IsAny<WtqAppOptions>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[TestMethod]
	public async Task StartOnly_AllowedToStart_IgnoresPreExistingWindowAndAttachesToNewOne()
	{
		// Arrange
		var opts = CreateOpts(AttachMode.StartOnly);
		var windows = new List<WtqWindow> { _preExisting };

		_windowService
			.Setup(s => s.FindWindowsAsync(opts, It.IsAny<CancellationToken>()))
			.ReturnsAsync(() => windows.ToList());

		// Starting the app makes a new window appear.
		_windowService
			.Setup(s => s.CreateAsync(opts, It.IsAny<CancellationToken>()))
			.Callback(() => windows.Add(_started))
			.Returns(Task.CompletedTask);

		// Act
		var window = await _resolver.GetWindowHandleAsync(opts, allowStartNew: true);

		// Assert
		Assert.AreSame(_started, window, "Should attach to the window that appeared after starting, not the pre-existing one.");

		_windowService.Verify(s => s.CreateAsync(opts, It.IsAny<CancellationToken>()), Times.Once);
		_startedWindows.Verify(s => s.SetWindowId(opts.Name!, _started.Id), Times.Once);
	}

	[TestMethod]
	public async Task StartOnly_RememberedWindowStillAround_ReattachesWithoutStarting()
	{
		// Arrange (e.g. WTQ was restarted, while the window it started earlier is still open)
		var opts = CreateOpts(AttachMode.StartOnly);

		_startedWindows
			.Setup(s => s.GetWindowId(opts.Name!))
			.Returns(_started.Id);

		_windowService
			.Setup(s => s.FindWindowsAsync(opts, It.IsAny<CancellationToken>()))
			.ReturnsAsync([_preExisting, _started]);

		// Act
		var window = await _resolver.GetWindowHandleAsync(opts, allowStartNew: true);

		// Assert
		Assert.AreSame(_started, window, "Should re-attach to the window that was started earlier.");

		_windowService.Verify(s => s.CreateAsync(It.IsAny<WtqAppOptions>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[TestMethod]
	public async Task StartOnly_RememberedWindowGone_ForgetsItAndDoesNotAttachToOthers()
	{
		// Arrange
		var opts = CreateOpts(AttachMode.StartOnly);

		_startedWindows
			.Setup(s => s.GetWindowId(opts.Name!))
			.Returns("gone");

		_windowService
			.Setup(s => s.FindWindowsAsync(opts, It.IsAny<CancellationToken>()))
			.ReturnsAsync([_preExisting]);

		// Act
		var window = await _resolver.GetWindowHandleAsync(opts, allowStartNew: false);

		// Assert
		Assert.IsNull(window);

		_startedWindows.Verify(s => s.RemoveWindowId(opts.Name!), Times.Once);
		_windowService.Verify(s => s.CreateAsync(It.IsAny<WtqAppOptions>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[TestMethod]
	public async Task StartOnly_WindowAppearsLate_KeepsLookingWithoutStartingAgain()
	{
		// Arrange
		var opts = CreateOpts(AttachMode.StartOnly);
		var windows = new List<WtqWindow> { _preExisting };

		_windowService
			.Setup(s => s.FindWindowsAsync(opts, It.IsAny<CancellationToken>()))
			.ReturnsAsync(() => windows.ToList());

		// Starting the app does not immediately result in a new window (e.g. Windows Terminal can take a while).
		_windowService
			.Setup(s => s.CreateAsync(opts, It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		// Act 1: start, but no window appears (yet).
		var window1 = await _resolver.GetWindowHandleAsync(opts, allowStartNew: true);

		// Act 2: periodic update (not allowed to start), still no window.
		var window2 = await _resolver.GetWindowHandleAsync(opts, allowStartNew: false);

		// Act 3: hotkey pressed again (allowed to start), should NOT start another instance while the first is pending.
		var window3 = await _resolver.GetWindowHandleAsync(opts, allowStartNew: true);

		// Now the window shows up.
		windows.Add(_started);

		// Act 4: periodic update picks it up.
		var window4 = await _resolver.GetWindowHandleAsync(opts, allowStartNew: false);

		// Assert
		Assert.IsNull(window1);
		Assert.IsNull(window2);
		Assert.IsNull(window3);
		Assert.AreSame(_started, window4);

		_windowService.Verify(s => s.CreateAsync(opts, It.IsAny<CancellationToken>()), Times.Once);
	}

	[TestMethod]
	public async Task FindOrStart_AttachesToPreExistingWindow()
	{
		// Arrange (sanity check that the default mode still grabs existing windows)
		var opts = CreateOpts(AttachMode.FindOrStart);

		_windowService
			.Setup(s => s.FindWindowsAsync(opts, It.IsAny<CancellationToken>()))
			.ReturnsAsync([_preExisting]);

		// Act
		var window = await _resolver.GetWindowHandleAsync(opts, allowStartNew: false);

		// Assert
		Assert.AreSame(_preExisting, window);
	}

	private static WtqAppOptions CreateOpts(AttachMode attachMode) =>
		new()
		{
			Name = "TestApp",
			FileName = "test",
			AttachMode = attachMode,
		};

	private static WtqWindow CreateWindow(string id)
	{
		var window = new Mock<WtqWindow>(MockBehavior.Strict);

		window.SetupGet(w => w.Id).Returns(id);
		window.Setup(w => w.ToString()).Returns(id);

		return window.Object;
	}
}
