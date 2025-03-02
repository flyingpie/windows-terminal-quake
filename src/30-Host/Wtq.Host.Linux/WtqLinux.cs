using Microsoft.Extensions.DependencyInjection;
using Namotion.Reflection;
using System;
using System.Linq.Expressions;
using Wtq.Configuration;
using Wtq.Host.Base;
using Wtq.Services.KWin;
using Wtq.Services.TrayIcon;
using Wtq.Utils;

namespace Wtq.Host.Linux;

public class WtqLinux : WtqHostBase
{
	protected override void ConfigureServices(IServiceCollection services)
	{
		Guard.Against.Null(services);

		// var y = new WtqSharedOptions();

		// var doc = SystemExtensions.GetMemberDoc(() => y.AttachMode);

		services
			.AddTrayIcon()
			.AddKWin();
	}
}