using Photino.NET;
using System.Drawing;
using System;
using System.IO;
using System.Text;

namespace Wtq.Host.Windows;

public static class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		// Window title declared here for visibility
		string windowTitle = "Photino for .NET Demo App";

		// Creating a new PhotinoWindow instance with the fluent API
		var window = new PhotinoWindow()
			.SetTitle(windowTitle)
			// Resize to a percentage of the main monitor work area
			.SetUseOsDefaultSize(false)
			.SetSize(new Size(1024, 800))
			// Center window in the middle of the screen
			.Center()
			// Users can resize windows by default.
			// Let's make this one fixed instead.
			.SetResizable(false)
			.RegisterCustomSchemeHandler("app", (object sender, string scheme, string url, out string contentType) =>
			{
				contentType = "text/javascript";
				return new MemoryStream(Encoding.UTF8.GetBytes(@"
                        (() =>{
                            window.setTimeout(() => {
                                alert(`🎉 Dynamically inserted JavaScript.`);
                            }, 1000);
                        })();
                    "));
			})
			// Most event handlers can be registered after the
			// PhotinoWindow was instantiated by calling a registration 
			// method like the following RegisterWebMessageReceivedHandler.
			// This could be added in the PhotinoWindowOptions if preferred.
			.RegisterWebMessageReceivedHandler((object sender, string message) =>
			{
				var window = (PhotinoWindow)sender;

				// The message argument is coming in from sendMessage.
				// "window.external.sendMessage(message: string)"
				string response = $"Received message: \"{message}\"";

				// Send a message back the to JavaScript event handler.
				// "window.external.receiveMessage(callback: Function)"
				window.SendWebMessage(response);
			})
			.Load("wwwroot/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.

		window.WaitForClose(); // Starts the application event loop
	}
}