using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Blazor.Fluxor;

namespace ReduxDevToolsIntegration.Client
{
	public class Program
	{
		static void Main(string[] args)
		{
			var serviceProvider = new BrowserServiceProvider(services =>
			{
				services.AddFluxor(options => options
					.UseDependencyInjection(typeof(Program).Assembly)
					.AddMiddleware<Blazor.Fluxor.ReduxDevTools.ReduxDevToolsMiddleware>()
					.AddMiddleware<Blazor.Fluxor.Routing.RoutingMiddleware>()
					);
			});

			new BrowserRenderer(serviceProvider).AddComponent<App>("app");
		}
	}
}
