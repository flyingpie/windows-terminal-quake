using Wtq.Services.TrayIcon;
using Wtq;
using Wtq.Services.UI;
using Wtq.Services.KWin;
using Wtq.Utils;

namespace Piper.Host.BlazorServer;

public static class Program
{
	public static async Task Main(string[] args)
	{
		Log.Configure();

		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services
			//.AddTrayIcon()
			.AddUI()
			.AddWtqCore()
			.AddKWin()
			;

		builder.Services.AddRazorPages();
		builder.Services.AddServerSideBlazor();

		var app = builder.Build();

		app.UseStaticFiles();

		app.UseRouting();

		app.MapBlazorHub();
		app.MapFallbackToPage("/_Host");

		app.Run();
	}
}