using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wtq.Services.Win32.Extensions;
using Wtq.Services.Win32.Native;

namespace Wtq.Services.Win32;

public static class Program
{
	public static void Main(string[] args)
	{




		//User32.GetWin32Windows();
		return;

		//var p = Process.GetProcesses();
		//var w = User32
		//	.FindWindows(null)
		//	.Select(h => new ProcTest
		//	{
		//		Handle = h,
		//		Title = User32.GetWindowText(h),
		//		Proc = User32.GetProcessHandleFromHwnd(h),
		//		//Pid = User32.GetWindowThreadProcessId(h, )
		//	})
		//	.Where(h => !string.IsNullOrWhiteSpace(h.Title))
		//	.OrderBy(h => h.Title)
		//	.ToList();

		//var list = new List<Process>();
		//foreach (var hh in w)
		//{
		//	try
		//	{
		//		var threadId = User32.GetWindowThreadProcessId(hh.Handle, out var processIdentifier);

		//		hh.ThreadId = threadId;
		//		hh.ProcessId = processIdentifier;

		//		hh.Pid = Kernel32.GetProcessId(processIdentifier);

		//		//list.Add(p.FirstOrDefault(pp => pp.Handle == hh.Proc));
		//	}
		//	catch { }
		//}


		//var res = new List<WtqWindow>();
		//foreach (var proc in Process.GetProcesses())
		//{
		//	var x2 = w.Where(xxx => xxx.Title.Contains("WhatsApp", StringComparison.OrdinalIgnoreCase)).ToList();
		//	//var h = w.FirstOrDefault(hx => hx.);


		//	if (!proc.TryGetHasExited(out var hasExited) || hasExited)
		//	{
		//		continue;
		//	}

		//	if (!proc.TryGetMainWindowHandle(out var mainWindowHandle) || mainWindowHandle == 0)
		//	{
		//		continue;
		//	}

		//	var wtqProcess = new Win32WtqWindow(proc);
		//	res.Add(wtqProcess);
		//}

		//var dbg = 2;
	}
}

class ProgramX
{

}



public class ProcTest
{
	public nint Handle;

	public int Proc;

	public string Title;

	public nint ThreadId;

	public nint ProcessId;

	public uint Pid;

	public override string ToString()
	{
		return $"[HANDLE:{Handle}] [Proc:{Proc}] [Pid:{ThreadId}] [OtherPid:{ProcessId}] {Title}";
	}
}