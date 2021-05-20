using System;
using System.Diagnostics;

namespace WindowsTerminalQuake.ProcessProviders
{
	public interface IProcessProvider
	{
		Process Get(string[] args);

		void OnExit(Action action);
	}
}