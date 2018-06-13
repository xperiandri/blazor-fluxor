using Blazor.Fluxor.ReduxDevTools;
using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;

namespace Blazor.Fluxor.Temporary
{
	/// <summary>
	/// If using Middleware that alters state then all components in your site should
	/// contain the line `@inherits Blazor.Fluxor.Temporary.FluxorComponent`
	/// </summary>
	/// <remarks>
	/// This class is temporary until Blazor has some kind of global StateHasChanged() call available
	/// </remarks>
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

		/// <summary>
		/// Calls StateHasChanged on all instances of FluxorComponent
		/// </summary>
		public static void AllStateHasChanged()
		{
			foreach (FluxorComponent component in AllComponents)
			{
#if DEBUG
                Console.WriteLine($"Executing {component.GetType()}.StateHasChanged()");
#endif
                component.StateHasChanged();
            }
        }

	}
}
