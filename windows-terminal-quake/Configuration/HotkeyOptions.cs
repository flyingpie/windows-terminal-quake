using System.Windows.Forms;
using Wtq.Native;

namespace Wtq.Configuration;

public class HotkeyOptions
{
	public Keys Key { get; set; }

	public KeyModifiers Modifiers { get; set; }
}