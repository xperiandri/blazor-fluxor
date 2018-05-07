using Blazor.Fluxor.ReduxDevTools;
using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;

namespace Blazor.Fluxor.Temporary
{
	// This class is temporary until Blazor has some kind of global StateHasChanged() call available
	// See https://github.com/aspnet/Blazor/issues/704
	public class FluxorComponent : BlazorComponent, IDisposable
	{
		private static HashSet<FluxorComponent> AllComponents = new HashSet<FluxorComponent>();

		public FluxorComponent()
		{
			AllComponents.Add(this);
		}

		public void Dispose()
		{
			AllComponents.Remove(this);
		}

		public static void AllStateHasChanged()
		{
			foreach (FluxorComponent component in AllComponents)
			{
				component.StateHasChanged();
				Console.WriteLine(component.GetType().Name + ".StateHasChanged()");
			}
		}

	}
}
