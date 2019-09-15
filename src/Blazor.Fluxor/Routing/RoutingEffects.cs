﻿using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Blazor.Fluxor.Routing
{
	public class RoutingEffects
	{
		private readonly NavigationManager NavigationManager;

		public RoutingEffects(NavigationManager navigationManager)
		{
			NavigationManager = navigationManager;
		}

		[Effect]
		public Task HandleGoAsync(Go action, IDispatcher dispatcher)
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
