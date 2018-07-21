using Microsoft.AspNetCore.Blazor.Browser.Rendering;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Blazor.Fluxor;

namespace FlightFinder.Client
{
	public class Program
	{
		static void Main(string[] args)
		{
			var serviceProvider = new BrowserServiceProvider(services =>
			{
				services.AddFluxor(x =>
					{
						x.UseDependencyInjection(typeof(Program).Assembly);
						x.AddMiddleware<Blazor.Fluxor.ReduxDevTools.ReduxDevToolsMiddleware>();
					}
				);
			});

			new BrowserRenderer(serviceProvider).AddComponent<Main>("body");
		}
	}
}
