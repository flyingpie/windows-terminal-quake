﻿using Microsoft.Extensions.DependencyInjection;

namespace Wtq.Services.UI;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddUI(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services
			.AddHostedService<WtqUI>();
	}
}