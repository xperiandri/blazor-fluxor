using Blazor.Fluxor.AutoDiscovery;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Blazor.Fluxor.Routing
{
	public class GoEffect
	{
		private readonly NavigationManager NavigationManager;

		public GoEffect(NavigationManager navigationManager)
		{
			NavigationManager = navigationManager;
		}

		[Effect]
		public Task HandleAsync(Go action, IDispatcher dispatcher)
		{
			Uri fullUri = NavigationManager.ToAbsoluteUri(action.NewUri);
			if (fullUri.ToString() != NavigationManager.Uri)
			{
				// Only navigate if we are not already at the URI specified
				NavigationManager.NavigateTo(action.NewUri);
			}
			return Task.CompletedTask;
		}
	}
}
