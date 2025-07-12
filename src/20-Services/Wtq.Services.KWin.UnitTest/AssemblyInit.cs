using Wtq.Utils;

namespace Wtq.Services.KWin.UnitTest;

[TestClass]
public static class AssemblyInit
{
	[AssemblyInitialize]
	public static void Setup(TestContext ctx)
	{
		// Log.Configure();
	}
}