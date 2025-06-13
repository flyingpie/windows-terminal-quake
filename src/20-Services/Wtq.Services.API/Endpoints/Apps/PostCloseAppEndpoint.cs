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
		if (!string.IsNullOrWhiteSpace(appName))
		{
			var app = appRepo.GetByName(appName);

			if (app == null)
			{
				return BadRequest();
			}

			if (!app.IsOpen)
			{
				return BadRequest();
			}

			await app.CloseAsync().NoCtx();
		}
		else
		{
			var open = appRepo.GetOpen();

			if (open != null)
			{
				await open.CloseAsync().NoCtx();
			}
		}

		return Ok();
	}
}