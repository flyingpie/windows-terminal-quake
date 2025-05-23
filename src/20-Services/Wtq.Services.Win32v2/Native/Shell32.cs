namespace Wtq.Services.Win32v2.Native;

public static class Shell32
{
	private static ITaskbarList TaskbarList;

	static Shell32()
	{
		TaskbarList = (ITaskbarList)new CTaskbarList();
		TaskbarList.HrInit();
	}

	public static void SetTaskbarIconVisible(nint windowHandle, bool isVisible)
	{
		if (isVisible)
		{
			TaskbarList.AddTab(windowHandle);
		}
		else
		{
			TaskbarList.DeleteTab(windowHandle);
		}
	}
}