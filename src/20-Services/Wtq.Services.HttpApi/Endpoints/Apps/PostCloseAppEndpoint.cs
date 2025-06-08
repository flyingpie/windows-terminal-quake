using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Wtq.Services.HttpApi.Endpoints.Apps;

[ApiController]
[Route("apps/close")]
public class PostCloseAppEndpoint : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> ExecuteAsync(
		[FromServices] IWtqAppRepo appRepo,
		[FromQuery, Required] string appName)
	{
		var app = appRepo.GetByName(appName);

		if (app == null)
		{
			return BadRequest();
		}

		if (app.IsOpen)
		{
			return BadRequest();
		}

		await app.CloseAsync().NoCtx();

		return Ok();
	}
}