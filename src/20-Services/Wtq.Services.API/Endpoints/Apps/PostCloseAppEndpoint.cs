using Microsoft.AspNetCore.Mvc;

namespace Wtq.Services.API.Endpoints.Apps;

[ApiController]
[Route("apps/close")]
public class PostCloseAppEndpoint : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> ExecuteAsync(
		[FromServices] IWtqAppRepo appRepo,
		[FromQuery] string? appName)
	{
		// If a specific app name was specified, only close that.
		if (!string.IsNullOrWhiteSpace(appName))
		{
			var app = appRepo.GetByName(appName);

			// Make sure the app exists.
			if (app == null)
			{
				return BadRequest();
			}

			// If the app is already open, don't close it again.
			if (!app.IsOpen)
			{
				return BadRequest();
			}

			// Close the app.
			await app.CloseAsync().NoCtx();
		}

		// If no specific app name was specified, just close whatever app is currently open.
		else
		{
			var open = appRepo.GetOpen();

			// Make sure an open app was found.
			if (open == null)
			{
				return BadRequest();
			}

			// If an open app was found, close it.
			await open.CloseAsync().NoCtx();
		}

		return Ok();
	}
}