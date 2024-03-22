using Wtq.Core.Data;
using Wtq.WinForms.Native;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace Wtq.WinForms;

internal static class Mapping
{
	internal static WtqKeys ToWtqKeys(this System.Windows.Forms.Keys key)
	{
		// switch (keyCode)
		// {
		// case Keys.D0: return WtqKeys.D0;
		// case Keys.D1: return WtqKeys.D1;
		// case Keys.D2: return WtqKeys.D2;
		// case Keys.D3: return WtqKeys.D3;
		// case Keys.D4: return WtqKeys.D4;
		// case Keys.D5: return WtqKeys.D5;
		// case Keys.D6: return WtqKeys.D6;
		// case Keys.D7: return WtqKeys.D7;
		// case Keys.D8: return WtqKeys.D8;
		// case Keys.D9: return WtqKeys.D9;
		// case Keys.Oemtilde: return WtqKeys.Oemtilde;

		// case KeyCode.VcQ: return WtqKeys.Q;

		// default:
		// return WtqKeys.None;
		// }
		var res = (WtqKeys)key;

		return res;
	}

	internal static WtqKeyModifiers ToWtqKeyModifiers(this KeyModifiers mask)
	{
		return (WtqKeyModifiers)mask;
	}
}