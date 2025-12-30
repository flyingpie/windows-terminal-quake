using System.Runtime.InteropServices;
using System.Security;

namespace Wtq.Services.Win32.Native;

/// <summary>
/// Provides access to the Windows taskbar.
/// </summary>
[ComImport]
[SuppressUnmanagedCodeSecurity]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("56FDF342-FD6D-11d0-958A-006097C9A090")]
[CoClass(typeof(CTaskbarList))]
public interface ITaskbarList
{
	/// <summary>
	/// Initializes the taskbar list object. This method must be called before any other <see cref="ITaskbarList"/> methods can be called.
	/// </summary>
	void HrInit();

	/// <summary>
	/// Adds the taskbar icon for the window with the specified <paramref name="windowHandle"/>.
	/// </summary>
	void AddTab(nint windowHandle);

	/// <summary>
	/// Removes the taskbar icon for the window with the specified <paramref name="windowHandle"/>.
	/// </summary>
	void DeleteTab(nint windowHandle);
}