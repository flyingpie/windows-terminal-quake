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
		// Look up the requested app.
		var app = appRepo.GetByName(appName);

		// Make sure an app was found.
		if (app == null)
		{
			return BadRequest();
		}

		// Make sure the app is not open.
		if (app.IsOpen)
		{
			return BadRequest();
		}

		// If another app is already open, close it first.
		foreach (var open in appRepo.GetOpen())
		{
			await open.CloseAsync().NoCtx();
		}

		// Open the requested app.
		await app.OpenAsync();

		return Ok();
	}
}