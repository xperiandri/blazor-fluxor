using Blazor.Fluxor;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using MiddlewareSample.Client.Store.Middlewares.AnExample;

namespace MiddlewareSample.Client
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddFluxor(options => options
				.UseDependencyInjection(typeof(Startup).Assembly)
				.AddMiddleware<Blazor.Fluxor.Routing.RoutingMiddleware>() // So we can see route changes in the console
				.AddMiddleware<AnExampleMiddleware>()
			);
		}

		public void Configure(IBlazorApplicationBuilder app)
		{
			app.AddComponent<App>("app");
		}
	}
}
