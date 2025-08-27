using SharpHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wtq.Services.SharpHook;

public static class SharpHookExtensions
{
	public static string Describe(this KeyboardHookEventArgs a)
	{
		return
			$"Data.KeyChar:{a.Data.KeyChar};" +
			$"Data.KeyCode:{a.Data.KeyCode};" +
			$"Data.RawCode:{a.Data.RawCode};" +
			$"Data.RawKeyChar:{a.Data.RawKeyChar};" +
			$"RawEvent.Mask:{a.RawEvent.Mask};" +
			$"RawEvent.Type:{a.RawEvent.Type};";
	}
}