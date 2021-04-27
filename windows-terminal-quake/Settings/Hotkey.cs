using System.Windows.Forms;
using WindowsTerminalQuake.Native;

namespace WindowsTerminalQuake.Settings
{
	public class Hotkey
	{
		public Keys Key { get; set; }

		public KeyModifiers Modifiers { get; set; }
	}
}