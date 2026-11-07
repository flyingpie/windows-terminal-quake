// using System.Diagnostics;
// using System.Threading;
//
// namespace Wtq.Core.UnitTest.Utils;
//
// [TestClass]
// public class TaskUtilsTest
// {
// 	[TestMethod]
// 	public async Task Completes()
// 	{
// 		// Arrange
// 		var sw = Stopwatch.StartNew();
//
// 		// Act
// 		await TaskUtils.DelayWithNoExceptionAsync(200);
//
// 		// Assert
// 		Assert.IsInRange(200, 220, sw.ElapsedMilliseconds);
// 	}
//
// 	[TestMethod]
// 	public async Task Interrupted()
// 	{
// 		// Arrange
// 		var sw = Stopwatch.StartNew();
//
// 		// Act
// 		var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
// 		await TaskUtils.DelayWithNoExceptionAsync(200, cts.Token);
//
// 		// Assert
// 		Assert.IsInRange(99, 120, sw.ElapsedMilliseconds);
// 	}
// }
