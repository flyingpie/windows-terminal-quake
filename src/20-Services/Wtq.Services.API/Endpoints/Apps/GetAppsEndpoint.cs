using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Wtq.Services.HttpApi.Endpoints.Apps;

[ApiController]
[Route("apps")]
public class GetAppsEndpoint : ControllerBase
{
	[HttpGet]
	[Produces<ResponseVM>]
	public Task<IActionResult> ExecuteAsync(
		[FromServices] IWtqAppRepo appRepo)
	{
		var apps = appRepo.GetAll();

		return Task.FromResult<IActionResult>(Ok(new ResponseVM(apps)));
	}

	public class ResponseVM(IEnumerable<WtqApp> apps)
	{
		public List<WtqAppVM> Apps { get; set; } = apps.Select(a => new WtqAppVM(a)).ToList();
	}

	public class WtqAppVM(WtqApp app)
	{
		public string Name => app.Name;

		public bool IsAttached => app.IsAttached;

		public bool IsOpen => app.IsOpen;
	}
}