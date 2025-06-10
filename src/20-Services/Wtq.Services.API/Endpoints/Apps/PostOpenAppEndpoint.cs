using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Wtq.Services.API.Endpoints.Apps;

[ApiController]
[Route("apps/open")]
public class PostOpenAppEndpoint : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> ExecuteAsync(
		[FromServices] IWtqAppRepo appRepo,
		[FromQuery, Required] string appName)
	{
		var openApp = appRepo.GetOpen();
		var app = appRepo.GetByName(appName);

		if (app == null)
		{
			return BadRequest();
		}

		if (app.IsOpen)
		{
			return BadRequest();
		}

		if (openApp != null)
		{
			await openApp.CloseAsync().NoCtx();
		}

		await app.OpenAsync();

		return Ok();
	}
}