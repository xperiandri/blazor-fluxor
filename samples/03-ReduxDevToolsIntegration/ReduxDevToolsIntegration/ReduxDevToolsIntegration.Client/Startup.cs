using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using Blazor.Fluxor;

namespace ReduxDevToolsIntegration.Client
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddFluxor(options => options
				.UseDependencyInjection(typeof(Startup).Assembly)
				.AddMiddleware<Blazor.Fluxor.ReduxDevTools.ReduxDevToolsMiddleware>()
				.AddMiddleware<Blazor.Fluxor.Routing.RoutingMiddleware>()
			);
		}

		public void Configure(IBlazorApplicationBuilder app)
		{
			app.AddComponent<App>("app");
		}
	}
}
