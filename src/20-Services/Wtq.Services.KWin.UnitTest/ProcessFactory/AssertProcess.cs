using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Wtq.Services.KWin.UnitTest.ProcessFactory;

public static class AssertProcess
{
	public static void Equals(
		Process process,
		string fileName,
		string workingDir,
		Collection<string> arguments)
	{
		// Filename
		Assert.AreEqual(fileName, process.StartInfo.FileName);

		// Working dir
		Assert.AreEqual(workingDir, process.StartInfo.WorkingDirectory);

		// Arguments
		Assert.IsNotNull(process.StartInfo.ArgumentList);
		Assert.AreEqual(arguments.Count, process.StartInfo.ArgumentList.Count);
		for (var i = 0; i < arguments.Count; i++)
		{
			Assert.AreEqual(arguments[i], process.StartInfo.ArgumentList[i]);
		}
	}
}