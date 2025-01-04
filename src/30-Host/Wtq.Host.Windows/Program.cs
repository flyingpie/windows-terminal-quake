using Photino.NET;
using System.Drawing;
using System;
using System.IO;
using System.Text;

namespace Wtq.Host.Windows;

public static class Program
{
	[STAThread]
	public static void Main(string[] args) => new WtqWin32().Run(args);
}